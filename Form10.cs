using System.Data;
using Microsoft.Data.SqlClient;
using WinFormsApp3;

namespace db
{
    public partial class Form10 : Form
    {
        int pricer = 0;
        int bookcounter = 0;
        string connection = Locator.GetConnectionString();
        public Form10()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);

            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            string curruser = "";

            using (SqlConnection conn = new SqlConnection(connection))
            {

                DataTable tb = new DataTable();
                tb.Columns.Add("user");
                tb.Columns.Add("name of the book");
                tb.Columns.Add("price of the book");

                string querry = $"SELECT iduser,bookname,price FROM saver1 ORDER BY iduser ";
                SqlCommand cmd = new SqlCommand(querry, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    if (curruser != null && curruser != reader["iduser"].ToString())
                    {
                        tb.Rows.Add("total", "total books: "+bookcounter, "total payment: "+pricer);


                        pricer = 0;
                        bookcounter = 0;
                    }
                    curruser = reader["iduser"].ToString();
                    tb.Rows.Add(
                       reader["iduser"],
                        reader["bookname"],
                        reader["price"]
                        );
                    pricer += Convert.ToInt32(reader["price"]);
                    bookcounter++;

                }
                tb.Rows.Add("total","total books: " + bookcounter, "total payment: " + pricer);
                dataGridView1.DataSource = tb;

                conn.Close();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Form3 form3 = new Form3();
            form3.Location = this.Location;
            form3.Size = this.Size;
            form3.StartPosition = FormStartPosition.Manual;
            form3.Show();

        }
    }
}
