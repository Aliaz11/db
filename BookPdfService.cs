using System.Data;
using Microsoft.Data.SqlClient;
using db.Configuration;

namespace db
{
    internal sealed record BookPdfInfo(int BookId, string Title, string FileName, long Length);

    /// <summary>
    /// Reads PDF metadata cheaply and streams the selected PDF only after the user chooses a
    /// destination. Catalogue queries deliberately exclude PdfData so browsing never loads every
    /// book file into memory.
    /// </summary>
    internal static class BookPdfService
    {
        internal static BookPdfInfo? GetInfo(int bookId)
        {
            using SqlConnection connection = new SqlConnection(AppSettings.ConnectionString);
            connection.Open();

            const string query = """
                SELECT [name], PdfFileName, DATALENGTH(PdfData) AS PdfLength
                FROM dbo.Books
                WHERE Id = @id AND PdfData IS NOT NULL AND DATALENGTH(PdfData) > 0
                """;

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = bookId;

            using SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
            if (!reader.Read())
            {
                return null;
            }

            string title = reader["name"] as string ?? "Book";
            string? storedFileName = reader["PdfFileName"] as string;
            long length = Convert.ToInt64(reader["PdfLength"]);
            return new BookPdfInfo(bookId, title, SafePdfFileName(storedFileName, title), length);
        }

        internal static async Task SavePdfAsync(
            int bookId,
            string destinationPath,
            CancellationToken cancellationToken = default)
        {
            string fullDestination = Path.GetFullPath(destinationPath);
            string? directory = Path.GetDirectoryName(fullDestination);
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new IOException("The selected download folder is not valid.");
            }

            Directory.CreateDirectory(directory);
            string temporaryPath = Path.Combine(
                directory,
                $".{Path.GetFileName(fullDestination)}.{Guid.NewGuid():N}.download");

            try
            {
                await using SqlConnection connection = new SqlConnection(AppSettings.ConnectionString);
                await connection.OpenAsync(cancellationToken);

                const string query = """
                    SELECT PdfData
                    FROM dbo.Books
                    WHERE Id = @id AND PdfData IS NOT NULL AND DATALENGTH(PdfData) > 0
                    """;

                await using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@id", SqlDbType.Int).Value = bookId;

                await using SqlDataReader reader = await command.ExecuteReaderAsync(
                    CommandBehavior.SequentialAccess | CommandBehavior.SingleRow,
                    cancellationToken);

                if (!await reader.ReadAsync(cancellationToken))
                {
                    throw new InvalidOperationException("This book does not have a downloadable PDF.");
                }

                using Stream source = reader.GetStream(0);
                await using (FileStream destination = new FileStream(
                    temporaryPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    await source.CopyToAsync(destination, 81920, cancellationToken);
                    await destination.FlushAsync(cancellationToken);
                }

                File.Move(temporaryPath, fullDestination, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        private static string SafePdfFileName(string? storedFileName, string title)
        {
            string candidate = Path.GetFileName(storedFileName ?? "").Trim();
            if (candidate.Length == 0)
            {
                candidate = title.Trim();
            }

            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                candidate = candidate.Replace(invalid, '_');
            }

            if (!candidate.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                candidate += ".pdf";
            }

            return candidate;
        }
    }
}
