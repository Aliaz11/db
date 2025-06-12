using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using WinFormsApp3;


namespace db
{
    public partial class Form4 : Form
    {
        public List<Book> books = new List<Book>();
        public Form4()
        {
            InitializeComponent();
        }








        private void button1_Click(object sender, EventArgs e)
        {


            try
            {
                byte[] images = null;
                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connection_string);
                sqlConnection.Open();
                string username = textBox1.Text;


                string query = $"SELECT username,password,image FROM Stu1 where username='{username}'";


                SqlCommand command = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();
                List<string> ali = new List<string>();


                while (reader.Read())
                {
                    ali.Add(reader["username"].ToString());
                    ali.Add(reader["password"].ToString());
              
                       // images = (byte[])reader["image"];
                    
              

                }
                if (username == "admin")
                {
                    if (ali[1] == textBox2.Text)
                    {

                        Form3 form3 = new Form3();

                        this.Hide();
                        form3.Show();



                    }
                }
                else if (ali[0] == username && ali[1] == textBox2.Text)
                {
                    Form9 form9 = new Form9(username);


                    this.Hide();

                    form9.Show();
                }





                else
                {
                    MessageBox.Show("the password or the user name was wrong");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }


   

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 form = new Form1();
            form.Show();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            byte[] imageBytes = Resource1.that;
            using (var ms = new System.IO.MemoryStream(imageBytes))
            {
                var image = System.Drawing.Image.FromStream(ms);
                this.BackgroundImage = image;
            }

            this.BackgroundImageLayout = ImageLayout.Stretch;



        }

        private void label4_Click(object sender, EventArgs e)
        {
            Form8 form8= new Form8();
            form8.Show();
            this.Close();
        }
    }
}

