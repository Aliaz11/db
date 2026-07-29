/*
    Adds optional downloadable PDF data to the book catalogue.
    Idempotent: safe to run more than once.
*/

SET NOCOUNT ON;
GO

IF COL_LENGTH('dbo.Books', 'PdfFileName') IS NULL
BEGIN
    ALTER TABLE dbo.Books ADD PdfFileName NVARCHAR(260) NULL;
    PRINT 'Added dbo.Books.PdfFileName.';
END
ELSE
BEGIN
    PRINT 'dbo.Books.PdfFileName already exists.';
END;
GO

IF COL_LENGTH('dbo.Books', 'PdfData') IS NULL
BEGIN
    ALTER TABLE dbo.Books ADD PdfData VARBINARY(MAX) NULL;
    PRINT 'Added dbo.Books.PdfData.';
END
ELSE
BEGIN
    PRINT 'dbo.Books.PdfData already exists.';
END;
GO

SELECT
    COUNT(*) AS TotalBooks,
    SUM(CASE WHEN PdfData IS NULL OR DATALENGTH(PdfData) = 0 THEN 0 ELSE 1 END)
        AS BooksWithPdf
FROM dbo.Books;
GO
