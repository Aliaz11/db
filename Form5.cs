using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using WinFormsApp3;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace db
{
    public partial class Form5 : Form
    {



        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            this.Close();
            form4.Show(); 
        }
    }
}
