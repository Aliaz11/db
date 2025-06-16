using System.Data;
using Microsoft.Data.SqlClient;
using WinFormsApp3;

namespace db
{

    public partial class Form9 : Form
    {
        int index;

        string connection1 = Locator.GetConnectionString();
        string idu;
        byte[] images;
        public Form9(string ids, byte[] images)
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
            string connection = Locator.GetConnectionString();
            this.idu = ids;
            this.images = images;
            using (MemoryStream ms = new MemoryStream(images))
            {
                pictureBox1.Image = Image.FromStream(ms);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            label3.Text = ids;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }
        public Form9()
        {
            InitializeComponent();

        }



        private void Form9_Load(object sender, EventArgs e)
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
                string query = $"SELECT * FROM saver1 WHERE iduser='{idu}'";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
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




                dataGridView1.DataSource = table;
                DataBaseCrud db = new DataBaseCrud(connection1);
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Height = 100;
                }
            }
        }





        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Form5 form5 = new Form5(idu,images);


            form5.Location = this.Location;
            form5.Size = this.Size;
            form5.StartPosition = FormStartPosition.Manual;
            form5.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DbCrudBook dbb = new DbCrudBook(connection1);
            dbb.delete(dataGridView1, index);
        }



        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                index = e.RowIndex;



            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 form = new Form1();
            form.Show();




        }
    }
}
