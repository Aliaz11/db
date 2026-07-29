/* ===========================================================================
   db -- minimal seed data.

   Run AFTER sql/schema.sql:

       sqlcmd -S "(localdb)\MSSQLLocalDB" -d Stu2 -i sql/seed.sql

   Idempotent: re-running it will not create duplicates.
   =========================================================================== */

SET NOCOUNT ON;
GO

/* ---------------------------------------------------------------------------
   The administrator account.

   The application decides who is an administrator purely by login name: the
   user whose Stu1.username is exactly 'admin' is routed to the management
   screen (Form3) instead of the shopper screen (Form9). There is no role
   column. Creating this row is therefore the whole of "make an admin".

   >>> PASSWORDS ARE NOT SET HERE. <<<

   Stu1.Password stores a PBKDF2 string produced by db.Security.PasswordHasher:

       PBKDF2$<iterations>$<base64 salt>$<base64 hash>

   That value can only be produced by the application, because the salt is
   random per user. This script therefore inserts a placeholder that is NOT a
   valid password and cannot be used to sign in. Set the real password through
   the app, either by:

     1. registering the account normally on the sign-up screen (Form2) -- delete
        the row created below first, so the username is free; or
     2. using "forgot password" on the login screen (Form4 -> Form8 -> Passchg),
        which emails a code to the address below and then writes a hashed
        password. This needs SMTP configured -- see the README.

   Do not paste a plaintext password into this file and do not "fix" it by
   putting one in the INSERT. A plaintext value in Stu1.Password is treated as a
   legacy credential by PasswordHasher.Verify and would let anyone with a copy
   of this repository log in as the administrator.
   --------------------------------------------------------------------------- */

IF NOT EXISTS (SELECT 1 FROM dbo.Stu1 WHERE username = 'admin')
BEGIN
    INSERT INTO dbo.Stu1 (firstname, lastname, phonenumber, Birthdate, Email, Gender, [Password], username, [image])
    VALUES
    (
        'Site',
        'Administrator',
        NULL,
        NULL,

        -- Change this to a mailbox you control before using the reset flow.
        'admin@example.invalid',

        NULL,

        -- Deliberately not a PBKDF2 string and deliberately not a usable
        -- plaintext password: PasswordHasher.Verify performs a fixed-time
        -- ordinal comparison against the literal below, and nobody can type a
        -- newline or the guard text by accident. Replace it via the app.
        '!! NOT SET -- set this password through the application !!',

        'admin',

        -- Form4 casts Stu1.image straight to byte[], so a NULL here throws on
        -- login. This is a 1x1 transparent PNG: valid for Image.FromStream,
        -- 67 bytes, and obviously a placeholder.
        0x89504E470D0A1A0A0000000D49484452000000010000000108060000001F15C4890000000A49444154789C63000100000500010D0A2DB40000000049454E44AE426082
    );

    PRINT 'Created the ''admin'' user. Its password is NOT set -- set it through the application.';
END
ELSE
BEGIN
    PRINT 'User ''admin'' already exists; left untouched.';
END;
GO

/* ---------------------------------------------------------------------------
   A couple of catalogue rows so the book grids are not empty on a fresh clone.

   `price` is stored as text and is read back with Convert.ToInt32 by
   DbCrudBook.inserter and Form10, so it must be a plain whole number with no
   currency symbol, no thousands separator and no decimal point. See the TODO
   on dbo.Books.price in schema.sql.
   --------------------------------------------------------------------------- */

IF NOT EXISTS (SELECT 1 FROM dbo.Books)
BEGIN
    INSERT INTO dbo.Books ([name], author, price, quantity, [Date], [image])
    VALUES
        ('The Pragmatic Programmer', 'Hunt & Thomas',    '45', 10, '2019-09-13', NULL),
        ('Clean Code',               'Robert C. Martin', '38',  7, '2008-08-01', NULL),
        ('Refactoring',              'Martin Fowler',    '52',  4, '2018-11-19', NULL);

    PRINT 'Inserted 3 sample books.';
END
ELSE
BEGIN
    PRINT 'dbo.Books already has rows; no sample data inserted.';
END;
GO

PRINT 'Seed complete.';
GO
