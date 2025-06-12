using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp3;

namespace db
{
    public partial class Form10 : Form
    {
        int pricer = 0;
        int bookcounter = 0;
        public Form10()
        {
            InitializeComponent();
            BackPhoto.BackSet(this);
        }


        private void Form10_Load(object sender, EventArgs e)
        {
            string curruser = null;

            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connectionString))
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
                        tb.Rows.Add("total", bookcounter, pricer);


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
                tb.Rows.Add("total", bookcounter, pricer);
                dataGridView1.DataSource = tb;

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Bold);

                dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;





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
