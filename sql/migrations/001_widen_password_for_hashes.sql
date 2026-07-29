/*
    001_widen_password_for_hashes.sql

    Run this against any database created BEFORE password hashing was introduced.
    Databases created from sql/schema.sql already have the correct width and are
    unaffected - the script is idempotent and safe to re-run either way.

    Why it is needed
    ----------------
    Stu1.Password was NVARCHAR(50), which was enough for the plaintext passwords
    the application used to store. A PBKDF2 record is 83 characters:

        PBKDF2$100000$<24 chars of base64 salt>$<44 chars of base64 hash>

    Against a 50-character column SQL Server raises

        Msg 2628: String or binary data would be truncated in ... column 'Password'

    which breaks registration and password reset outright, and makes the
    transparent re-hash of legacy plaintext rows fail silently (AuthService
    swallows that error on purpose so a failed upgrade cannot block a login
    that has already succeeded).

    200 characters leaves room to raise the iteration count or move to a longer
    hash without another migration.

    Existing plaintext passwords are NOT touched. They keep working and each one
    is re-hashed automatically the next time that user signs in successfully.
*/

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Stu1' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    RAISERROR('Table dbo.Stu1 was not found - is this the right database?', 16, 1);
    RETURN;
END

DECLARE @length INT = (
    SELECT CHARACTER_MAXIMUM_LENGTH
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Stu1' AND COLUMN_NAME = 'Password'
);

IF @length IS NULL
BEGIN
    RAISERROR('Column dbo.Stu1.Password was not found.', 16, 1);
    RETURN;
END

IF @length = -1
    PRINT 'No change needed: dbo.Stu1.Password is already NVARCHAR(MAX).';
ELSE IF @length >= 200
    PRINT 'No change needed: dbo.Stu1.Password is already NVARCHAR(' + CAST(@length AS VARCHAR(10)) + ').';
ELSE
BEGIN
    PRINT 'Widening dbo.Stu1.Password from NVARCHAR(' + CAST(@length AS VARCHAR(10)) + ') to NVARCHAR(200)...';

    -- The column is NOT NULL in the shipped schema; ALTER COLUMN drops that unless
    -- it is restated, so it is repeated here deliberately.
    ALTER TABLE dbo.Stu1 ALTER COLUMN Password NVARCHAR(200) NOT NULL;

    PRINT 'Done.';
END
GO

-- Verification: every row should still be readable, and the column should now fit a hash.
SELECT
    CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR(10)) AS PasswordColumnLength,
    (SELECT COUNT(*) FROM dbo.Stu1)               AS UserRows
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Stu1' AND COLUMN_NAME = 'Password';
GO
