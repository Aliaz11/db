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
using Microsoft.VisualBasic.ApplicationServices;
using WinFormsApp3;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace db
{
  
    public partial class Form5 : Form
    {
        int price1=0;
        string ids;

        public Form5()
        {
            InitializeComponent();
            BackPhoto.BackSet(this);
        }
        public Form5(string ids)
        {
            InitializeComponent();
            BackPhoto.BackSet(this);
            this.ids = ids;
        }


        private void Form5_Load(object sender, EventArgs e)
        {

            DataBaseCrud db1 = new DataBaseCrud("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf; Integrated Security = True");
            db1.selector(dataGridView1, this);






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

        private void button2_Click(object sender, EventArgs e)
        {

            var selectedBooks = new List<(string Name, string Author, string Price)>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                bool isChecked = Convert.ToBoolean(row.Cells["chk"].Value);
                if (isChecked)
                {
                    string bookName = row.Cells["name"].Value.ToString();
                    string author = row.Cells["author"].Value.ToString();
                    string price = row.Cells["price"].Value.ToString();
                     price1+= Convert.ToInt32(price);

                    selectedBooks.Add((bookName, author, price));
                }
            }
            MessageBox.Show("the price to pay: " + price1);

            if (selectedBooks.Count == 0)
            {
                MessageBox.Show("No books selected.");
                return;
            }

            try
            {
                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";

                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    conn.Open();

                    foreach (var book in selectedBooks)
                    {
                        string query = "INSERT INTO saver1 (iduser, bookname, author, price) VALUES (@user, @name, @auth, @price)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@user", ids);
                            cmd.Parameters.AddWithValue("@name", book.Name);
                            cmd.Parameters.AddWithValue("@auth", book.Author);
                            cmd.Parameters.AddWithValue("@price", book.Price);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("saved");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            Form9 form9 = new Form9();
            form9.ShowDialog();
        }
    }
}

