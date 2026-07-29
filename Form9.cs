using System.Data;
using Microsoft.Data.SqlClient;
using WinFormsApp3;

namespace db
{

    public partial class Form9 : Form
    {
        /// <summary>Row selected in the grid; -1 until the user clicks one.</summary>
        int index = -1;

        string connection1 = Locator.GetConnectionString();
        string idu = "";
        byte[] images = Array.Empty<byte>();

        public Form9(string ids, byte[] images)
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
            this.idu = ids ?? "";
            this.images = images ?? Array.Empty<byte>();

            ShowProfilePhoto();

            label3.Text = this.idu;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ModernTheme.Apply(this);
            AppFeatures.EnableGridTools(this, dataGridView1, GridToolMode.Shelf);
            ShelfExperience.Attach(this, dataGridView1, RemoveBookFromShelf, DownloadPdf);
        }

        public Form9()
        {
            InitializeComponent();
            ModernTheme.Apply(this);
            AppFeatures.EnableGridTools(this, dataGridView1, GridToolMode.Shelf);
            ShelfExperience.Attach(this, dataGridView1, RemoveBookFromShelf, DownloadPdf);
        }

        /// <summary>Loads the profile photo, tolerating a missing or unreadable image.</summary>
        private void ShowProfilePhoto()
        {
            if (images.Length == 0)
            {
                return;
            }

            try
            {
                using (MemoryStream ms = new MemoryStream(images))
                {
                    // Copied into a new Bitmap so the image stays valid after the stream is disposed.
                    pictureBox1.Image = new Bitmap(Image.FromStream(ms));
                }

                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch (ArgumentException)
            {
                // The stored bytes are not a readable image; leave the picture box empty.
            }
        }

        private void Form9_Load(object? sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connection1))
            {
                DataTable table = new DataTable();
                table.Columns.Add("id", typeof(int));
                table.Columns.Add("book's name", typeof(string));
                table.Columns.Add("author", typeof(string));
                table.Columns.Add("price", typeof(string));
                table.Columns.Add("imges", typeof(byte[]));
                table.Columns.Add("BookId", typeof(int));
                table.Columns.Add("HasPdf", typeof(bool));

                connection.Open();

                const string query = """
                    SELECT
                        saved.Id,
                        saved.bookname,
                        saved.author,
                        saved.price,
                        CASE
                            WHEN DATALENGTH(saved.[image]) > 0 THEN saved.[image]
                            ELSE catalogue.[image]
                        END AS [image],
                        catalogue.Id AS BookId,
                        CAST(CASE
                            WHEN catalogue.PdfData IS NULL
                                OR DATALENGTH(catalogue.PdfData) = 0 THEN 0
                            ELSE 1
                        END AS bit) AS HasPdf
                    FROM dbo.saver1 AS saved
                    OUTER APPLY
                    (
                        SELECT TOP (1) book.Id, book.[image], book.PdfData
                        FROM dbo.Books AS book
                        WHERE book.[name] = saved.bookname
                          AND book.author = saved.author
                        ORDER BY book.Id
                    ) AS catalogue
                    WHERE saved.iduser = @iduser
                    ORDER BY saved.Id
                    """;

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add("@iduser", SqlDbType.NVarChar, 200).Value = idu;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            table.Rows.Add(
                                Convert.ToInt32(reader["Id"]),
                                reader["bookname"] as string ?? "",
                                reader["author"] as string ?? "",
                                reader["price"]?.ToString() ?? "",
                                reader["image"] is byte[] cover ? cover : Array.Empty<byte>(),
                                reader["BookId"] == DBNull.Value ? DBNull.Value : reader["BookId"],
                                reader["HasPdf"] != DBNull.Value && Convert.ToBoolean(reader["HasPdf"])
                            );
                        }
                    }
                }

                dataGridView1.DataSource = table;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Height = 100;
                }
            }

            AppFeatures.RefreshGridTools(this, dataGridView1, GridToolMode.Shelf);
            ShelfExperience.Refresh(this, dataGridView1);
        }

        private async void DownloadPdf(int bookId)
        {
            BookPdfInfo? pdf;
            try
            {
                pdf = BookPdfService.GetInfo(bookId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"The PDF information could not be loaded: {ex.Message}",
                    "Download Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (pdf == null)
            {
                MessageBox.Show(
                    this,
                    "This shelved book does not have a downloadable PDF.",
                    "PDF Not Available",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using SaveFileDialog dialog = new SaveFileDialog
            {
                Title = $"Download {pdf.Title}",
                FileName = pdf.FileName,
                Filter = "PDF document (*.pdf)|*.pdf",
                DefaultExt = "pdf",
                AddExtension = true,
                OverwritePrompt = true,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Cursor previousCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            try
            {
                await BookPdfService.SavePdfAsync(bookId, dialog.FileName);
                MessageBox.Show(
                    this,
                    $"Saved {pdf.FileName} successfully.",
                    "Download Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"The PDF could not be downloaded: {ex.Message}",
                    "Download Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = previousCursor;
            }
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form5(idu, images));
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            if (index < 0)
            {
                MessageBox.Show(
                    "Please select the book you want to remove.",
                    "No Book Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            RemoveBookFromShelf(index);
        }

        private void RemoveBookFromShelf(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView1.Rows.Count)
            {
                return;
            }

            string title = dataGridView1.Rows[rowIndex].Cells.Count > 1
                ? dataGridView1.Rows[rowIndex].Cells[1].Value?.ToString() ?? "this book"
                : "this book";
            DialogResult confirmation = MessageBox.Show(
                $"Remove “{title}” from your shelf?",
                "Remove saved book",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            // The constructor parameter is the user id, not a connection string.
            DbCrudBook dbb = new DbCrudBook(idu);
            dbb.delete(dataGridView1, rowIndex);
            index = -1;
            AppFeatures.RefreshGridTools(this, dataGridView1, GridToolMode.Shelf);
            ShelfExperience.Refresh(this, dataGridView1);
        }

        private void dataGridView1_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                index = e.RowIndex;
            }
        }

        private void label3_Click(object? sender, EventArgs e)
        {

        }

        private void button3_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form1());
        }
    }
}
