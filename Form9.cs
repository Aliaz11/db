using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace db
{
    public partial class Form9 : Form
    {


        string connection1 = Locator.GetConnectionString();
        string idu;
        public Form9(string ids)
        {
            InitializeComponent();
            this.idu = ids;

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
                table.Columns.Add("book's name");
                table.Columns.Add("author");
                table.Columns.Add("price");



                connection.Open();
                string query = $"SELECT * FROM saver1 WHERE iduser='{idu}'";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {


                    table.Rows.Add(
       reader["bookname"].ToString(),
       reader["author"].ToString(),
       reader["price"].ToString()
   );

                }




                dataGridView1.DataSource = table;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Form5 form5 = new Form5(idu);
            form5.Show();
        }
    }
}
