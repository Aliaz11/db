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
        }

        public Form9()
        {
            InitializeComponent();
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
                table.Columns.Add("id");
                table.Columns.Add("book's name");
                table.Columns.Add("author");
                table.Columns.Add("price");
                table.Columns.Add("imges");

                connection.Open();

                const string query = "SELECT * FROM saver1 WHERE iduser = @iduser";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add("@iduser", SqlDbType.NVarChar, 200).Value = idu;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            table.Rows.Add(
                                reader["Id"].ToString(),
                                reader["bookname"].ToString(),
                                reader["author"].ToString(),
                                reader["price"].ToString(),
                                reader["image"]
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

            // The constructor parameter is the user id, not a connection string.
            DbCrudBook dbb = new DbCrudBook(idu);
            dbb.delete(dataGridView1, index);
            index = -1;
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
