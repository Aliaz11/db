using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using WinFormsApp3;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace db
{
    
    public partial class Form6 : Form
    {
        int index;
        string id;


        public Form6(List<Book> books)
        {
            InitializeComponent();


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form6_Load(object sender, EventArgs e)
        {
            try
            {
                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";

                using (SqlConnection sqlConnection = new SqlConnection(connection_string))
                {
                    sqlConnection.Open();

                    string query = "SELECT * FROM Books";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = dt;
                        if (dataGridView1.Columns.Contains("image"))
                        {
                            DataGridViewImageColumn imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["image"];
                            imageColumn.ImageLayout = DataGridViewImageCellLayout.Stretch;
                        }

                        dataGridView1.RowTemplate.Height = 100;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }















        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Form3 form1 = new Form3();
            form1.Location = this.Location;
            form1.Size = this.Size;
            form1.StartPosition = FormStartPosition.Manual;
            form1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {


        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DataGridViewRow newdata = dataGridView1.Rows[index];
            string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";
            using (SqlConnection sqlConnection = new SqlConnection(connection_string))
            {
                sqlConnection.Open();





               
                string query = $"DELETE FROM Books WHERE Id = {id}";

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {


                 







                    command.ExecuteNonQuery();
                    sqlConnection.Close();


                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[index];
            id = row.Cells[0].Value.ToString();
            textBox1.Text = row.Cells[1].Value.ToString();
            textBox2.Text = row.Cells[2].Value.ToString();
            textBox3.Text = row.Cells[3].Value.ToString();


        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            DataGridViewRow newdata = dataGridView1.Rows[index];
            newdata.Cells[1].Value = textBox1.Text;
            newdata.Cells[2].Value = textBox2.Text;
            newdata.Cells[3].Value = textBox3.Text;
            string name = textBox1.Text;
            string author = textBox2.Text;
            string price = textBox3.Text;
            DataGridViewRow row = dataGridView1.Rows[index];
            string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";
            using (SqlConnection sqlConnection = new SqlConnection(connection_string))
            {
                sqlConnection.Open();
          




                 id = row.Cells[0].Value.ToString();
                string query = $"UPDATE Books SET name =@name, author =@author,price=@price WHERE ID = {id}";

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {


                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@author", author);

                    command.Parameters.AddWithValue("@price", price);








                    command.ExecuteNonQuery();
                    sqlConnection.Close();


                }
            }
        }
    }
}
