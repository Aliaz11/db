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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace db
{
    public partial class Form7 : Form
    {
        string connection = Locator.GetConnectionString();
        public Form7()
        {
            InitializeComponent();
            BackPhoto.BackSet(this);
        }
        public byte[] getphoto()
        {
            MemoryStream stream = new MemoryStream();
            pictureBox1.Image.Save(stream, pictureBox1.Image.RawFormat);
            return stream.GetBuffer();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string author = textBox2.Text;
            string price = textBox3.Text;
            ;

            byte[] imageBytes = null;
            if (pictureBox1.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    imageBytes = ms.ToArray();
                }
            }

            string query = $"INSERT INTO Books(name,author,price,image)VALUES(@name,@author,@price,@image)";
            using (SqlConnection sqlconnection2 = new SqlConnection(connection))
            {
                sqlconnection2.Open();

                SqlCommand command2 = new SqlCommand(query, sqlconnection2);
                command2.Parameters.AddWithValue("@name", name);
                command2.Parameters.AddWithValue("@author", author);
                command2.Parameters.AddWithValue("@price", price);
                command2.Parameters.AddWithValue("@image", getphoto());

                command2.ExecuteNonQuery();
                sqlconnection2.Close();


            }
            textBox1.Text = textBox2.Text = textBox3.Text = "";
            pictureBox1.Image = null;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);



            }



        }


    }
}