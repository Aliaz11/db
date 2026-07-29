/* ===========================================================================
   db -- bookstore / user-management sample
   Schema creation script for the LocalDB database attached as Stu2.mdf.

   The repository ships the .mdf but never shipped a schema, so this script was
   reverse-engineered from the data-access code:
       DataBaseCrud.cs, DbCrudBook.cs, Form4.cs, Form7.cs, Form9.cs, Form10.cs,
       PasswordUpdator.cs, LabelValidator.cs, Book.cs, IUser.cs

   The script is idempotent: run it as many times as you like.

   Usage (from the repository root):

       sqlcmd -S "(localdb)\MSSQLLocalDB" -d Stu2 -i sql/schema.sql

   or, to build a brand new database file first:

       sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE Stu2 ON (NAME='Stu2', FILENAME='C:\path\to\repo\Stu2.mdf')"
       sqlcmd -S "(localdb)\MSSQLLocalDB" -d Stu2 -i sql/schema.sql

   Column names, including their casing, are copied verbatim from the SQL text
   in the C# code. Do not "tidy" them without changing the call sites -- the
   readers index by name (reader["bookname"], row.Cells["quantity"], ...).
   =========================================================================== */

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ---------------------------------------------------------------------------
   Stu1 -- application users.
   Written by  DataBaseCrud.insert / DataBaseCrud.update / PasswordUpdator.
   Read by     DataBaseCrud.selector(ListView), Form4 (login),
               LabelValidator.selectoring (uniqueness pre-check).
   --------------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Stu1' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Stu1
    (
        Id           INT             IDENTITY(1,1) NOT NULL,

        firstname    NVARCHAR(50)    NOT NULL,
        lastname     NVARCHAR(50)    NOT NULL,

        -- LabelValidator requires 10-11 characters *when supplied*; a user may
        -- register with an email instead, in which case Form2 sends "".
        phonenumber  NVARCHAR(20)    NULL,

        -- TODO: should be DATE. It is NVARCHAR today because Form2 writes
        --       user.BirthDate.ToString() (a full, culture-dependent DateTime
        --       string) and Form3 writes dateTimePicker1.Value.ToString("yyyy-MM-dd").
        --       Two different formats land in the same column. Fix the C# to pass
        --       a DateTime parameter before narrowing this type.
        Birthdate    NVARCHAR(50)    NULL,

        -- 254 is the maximum length of an RFC 5321 address. Nullable, and "" is
        -- also stored (Form2 sends an empty string when no email box was added).
        Email        NVARCHAR(254)   NULL,

        -- "male"/"female" from Form2, "Male"/"Female" from Form3. The default
        -- case-insensitive collation makes those equal; do not switch this
        -- database to a case-sensitive collation without normalising the values.
        -- TODO: CHECK (Gender IN ('male','female','')) once the casing is unified.
        Gender       NVARCHAR(10)    NULL,

        -- PBKDF2$<iterations>$<base64 salt>$<base64 hash>
        --   "PBKDF2$"                7
        --   iterations (6-7 digits)  7
        --   "$" + base64(16 bytes)   25
        --   "$" + base64(32 bytes)   45
        --                          ---- ~84 characters today.
        -- 200 leaves room for a larger salt/key or a different KDF prefix later.
        -- Legacy rows may still contain a plaintext password; PasswordHasher.Verify
        -- detects that and the login path re-hashes on the next successful sign-in.
        Password     NVARCHAR(200)   NOT NULL,

        username     NVARCHAR(50)    NOT NULL,

        -- Profile picture, raw bytes of whatever the user picked in the
        -- OpenFileDialog. Form4 casts reader["image"] straight to byte[], so a
        -- NULL here will throw on login -- see the seed script.
        [image]      VARBINARY(MAX)  NULL,

        CONSTRAINT PK_Stu1 PRIMARY KEY CLUSTERED (Id)
    );
END;
GO

-- Login name must be unique. LabelValidator.selectoring only checks this in
-- application code, by scanning every row; the index is what actually enforces it.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Stu1_username' AND object_id = OBJECT_ID('dbo.Stu1'))
BEGIN
    CREATE UNIQUE INDEX UX_Stu1_username ON dbo.Stu1 (username);
END;
GO

-- Email is optional, and "not provided" is persisted as an empty string rather
-- than NULL, so the uniqueness constraint has to be filtered or every second
-- phone-only registration would fail.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Stu1_Email' AND object_id = OBJECT_ID('dbo.Stu1'))
BEGIN
    CREATE UNIQUE INDEX UX_Stu1_Email
        ON dbo.Stu1 (Email)
        WHERE Email IS NOT NULL AND Email <> '';
END;
GO

/* ---------------------------------------------------------------------------
   Books -- the catalogue.
   Written by  DbCrudBook.insert (Form7), DataBaseCrud.updateBase (Form6).
   Read by     DataBaseCrud.selector(DataGridView, Form) -> Form5 / Form6.
   --------------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Books' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Books
    (
        ID          INT             IDENTITY(1,1) NOT NULL,

        [name]      NVARCHAR(200)   NOT NULL,
        author      NVARCHAR(200)   NOT NULL,

        -- TODO: should be DECIMAL(10,2). It is NVARCHAR because Book.price is a
        --       `string` in C# and is bound with AddWithValue, and because
        --       DbCrudBook.inserter copies the value verbatim into saver1.price
        --       and then compares the two with `price = @price` -- a string
        --       comparison, so '5' and '5.00' are different prices.
        --       Readers (Book.PriceValue, Form10.ParsePrice) parse leniently and
        --       fall back to 0, so a malformed value is silently free rather
        --       than an error.
        price       NVARCHAR(50)    NOT NULL,

        -- TODO: Book.quantity and DataBaseCrud.updateBase both use `decimal`
        --       (NumericUpDown.Value). INT is correct for a stock count and works
        --       today because the NumericUpDown has DecimalPlaces = 0, but a
        --       fractional value would be rejected. Change the C# to int rather
        --       than widening this column.
        quantity    INT             NOT NULL CONSTRAINT DF_Books_quantity DEFAULT (0),

        -- TODO: should be DATE. Today two different producers write it:
        --         Form7            -> dateTimePicker1.Value.ToString()  (a string)
        --         updateBase       -> datatime.Value                    (a DateTime)
        --       NVARCHAR is the only type that accepts both without a conversion
        --       error, which is why it is used here. Readers do DateTime.TryParse.
        [Date]      NVARCHAR(50)    NULL,

        -- Cover art. DataBaseCrud.selector casts the grid column to a
        -- DataGridViewImageColumn, so this must stay VARBINARY.
        [image]     VARBINARY(MAX)  NULL,

        -- Downloadable source document. Catalogue list queries intentionally exclude
        -- PdfData and fetch it with SequentialAccess only after a user chooses a save path.
        PdfFileName NVARCHAR(260)   NULL,
        PdfData     VARBINARY(MAX)  NULL,

        CONSTRAINT PK_Books PRIMARY KEY CLUSTERED (ID)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Books_name_author' AND object_id = OBJECT_ID('dbo.Books'))
BEGIN
    CREATE INDEX IX_Books_name_author ON dbo.Books ([name], author);
END;
GO

/* ---------------------------------------------------------------------------
   saver1 -- the per-user basket of saved books.
   Written by  DbCrudBook.inserter (Form5 "buy"/"save" button).
   Read by     Form9 (one user's basket), Form10 (admin sales report).
   Deleted by  DbCrudBook.delete (Form9).

   NOTE: `iduser` is NOT a foreign key and does NOT hold Stu1.Id. Form4 passes
   AuthenticatedUser.UserName through Form9 -> Form5 -> new DbCrudBook(ids), so
   this column contains Stu1.username. The column name is a leftover from an
   earlier design and the DbCrudBook doc comment still calls it "the Stu1.Id".
   --------------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'saver1' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.saver1
    (
        Id          INT             IDENTITY(1,1) NOT NULL,

        -- Holds a Stu1.username value, so this stays the same width as
        -- Stu1.username. Note that Form9 binds its lookup parameter as
        -- SqlDbType.NVarChar with size 200; that is wider than the column but
        -- harmless (same type, same collation, the index seek still applies).
        -- TODO: replace with `userId INT NOT NULL REFERENCES dbo.Stu1(Id)` and
        --       carry the id (not the login name) through the form
        --       constructors. DbCrudBook's parameter is already *named* `id`.
        iduser      NVARCHAR(50)    NOT NULL,

        -- Denormalised snapshot of the Books row at the time it was saved.
        -- TODO: replace bookname/author/price/image with bookId INT REFERENCES
        --       dbo.Books(ID). The copy exists only because DbCrudBook.inserter
        --       reads the values out of the DataGridView instead of the database.
        bookname    NVARCHAR(200)   NOT NULL,
        author      NVARCHAR(200)   NOT NULL,

        -- Must stay the same type and width as Books.price: DbCrudBook.inserter
        -- dedupes with "... AND price = @price" against the text copied out of
        -- the grid. Form10 parses this column with decimal.TryParse (invariant
        -- culture first, then the current culture) and treats a failure as 0.
        price       NVARCHAR(50)    NOT NULL,

        [image]     VARBINARY(MAX)  NULL,

        CONSTRAINT PK_saver1 PRIMARY KEY CLUSTERED (Id)
    );
END;
GO

-- Form9 filters by iduser; Form10 orders by it.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_saver1_iduser' AND object_id = OBJECT_ID('dbo.saver1'))
BEGIN
    CREATE INDEX IX_saver1_iduser ON dbo.saver1 (iduser);
END;
GO

-- DbCrudBook.inserter already refuses to insert a duplicate
-- (iduser, bookname, author, price) row, but it does so with a SELECT COUNT(*)
-- followed by an INSERT, which is racy. This makes the rule real.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_saver1_user_book' AND object_id = OBJECT_ID('dbo.saver1'))
BEGIN
    CREATE UNIQUE INDEX UX_saver1_user_book
        ON dbo.saver1 (iduser, bookname, author, price);
END;
GO

PRINT 'Schema check complete: dbo.Stu1, dbo.Books, dbo.saver1.';
GO
