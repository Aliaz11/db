using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using db;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.PortableExecutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.ApplicationServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinFormsApp3
{
    public partial class Form3 : Form
    {
        public List<Book> books = new List<Book>();
        public Form3()
        {
            InitializeComponent();

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("ID", 100);
            listView1.Columns.Add("First Name", 100);
            listView1.Columns.Add("Last Name", 100);
            listView1.Columns.Add("Phone Number", 120);
            listView1.Columns.Add("Birth Date", 100);
            listView1.Columns.Add("Email", 150);
            listView1.Columns.Add("Gender", 80);
            listView1.Columns.Add("password", 80);
        }

  
        private void label2_Click(object sender, EventArgs e)
        {

        }
        List<string> list = new List<string>();
        void refresh()
        {

            try
            {
                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connection_string);
                sqlConnection.Open();
                string query = "SELECT * FROM Stu1";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();
                listView1.Items.Clear();

                while (reader.Read())
                {

                    ListViewItem item = new ListViewItem(reader["Id"].ToString());

                    item.SubItems.Add(reader["firstname"].ToString());

                    item.SubItems.Add(reader["lastname"].ToString());
                    item.SubItems.Add(reader["phonenumber"].ToString());
                    item.SubItems.Add(reader["birthdate"].ToString());
                    item.SubItems.Add(reader["email"].ToString());
                    item.SubItems.Add(reader["gender"].ToString());
                    item.SubItems.Add(reader["password"].ToString());





                    listView1.Items.Add(item);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        private void Form3_Load(object sender, EventArgs e)
        {
            this.BackgroundImageLayout = ImageLayout.Stretch;
            {
                byte[] imageBytes = Resource1.that;
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    var image = System.Drawing.Image.FromStream(ms);
                    this.BackgroundImage = image;
                }



            }
            string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();
            string query = "SELECT * FROM Stu1";
            SqlCommand command = new SqlCommand(query, sqlConnection);




            refresh();




        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("select a row .");
                return;
            }
            try
            {

                string firstname = string.IsNullOrWhiteSpace(textBox1.Text) ? listView1.SelectedItems[0].SubItems[0].Text : textBox1.Text;
                string lastname = string.IsNullOrWhiteSpace(textBox2.Text) ? listView1.SelectedItems[0].SubItems[1].Text : textBox1.Text;
                string phonenumber = string.IsNullOrWhiteSpace(textBox3.Text) ? listView1.SelectedItems[0].SubItems[2].Text : textBox1.Text;


                string phonenumber1 = listView1.SelectedItems[0].SubItems[2].Text;
                string gender = "";
                string Birthdate = dateTimePicker1.Value.ToString();
                string password = textBox4.Text;
                if (radioButton1.Checked)
                {
                    gender = "male";
                }
                else if (radioButton2.Checked)
                {
                    gender = "female";

                }





                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";
                using (SqlConnection sqlConnection = new SqlConnection(connection_string))
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        string phone = item.SubItems[2].Text;
                        foreach (ListViewItem i in listView1.SelectedItems)
                        {
                            string id = item.SubItems[0].Text;
                            string query = $"UPDATE Stu1 SET firstname = @firstname, lastname = @lastname, phonenumber = @phonenumber,gender=@gender,Birthdate=@Birthdate,password=@password WHERE ID = {id}";

                            using (SqlCommand command = new SqlCommand(query, sqlConnection))
                            {


                                command.Parameters.AddWithValue("@firstname", firstname);
                                command.Parameters.AddWithValue("@gender", gender);

                                command.Parameters.AddWithValue("@lastname", lastname);
                                command.Parameters.AddWithValue("@phonenumber", phonenumber1);
                                command.Parameters.AddWithValue("@Birthdate", Birthdate);
                                command.Parameters.AddWithValue("@password", password);



                                textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = "";
                                if (command.ExecuteNonQuery() > 0)
                                {
                                    refresh();
                                }
                            }
                        }
                        sqlConnection.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 form = new Form1();
            form.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {


            if (listView1.SelectedItems.Count > 0)
            {
                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connection_string);
                sqlConnection.Open();

                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    string id = item.SubItems[0].Text;


                    string query = $"DELETE FROM Stu1 WHERE Id = {id}";
                    SqlCommand cmd = new SqlCommand(query, sqlConnection);


                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        listView1.Items.Remove(item);
                    }

                }

                sqlConnection.Close();
            }
            else
            {
                MessageBox.Show("Please select a row to remove.");
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6(books);
            form6.Location = this.Location;
            form6.Size = this.Size;
            form6.StartPosition = FormStartPosition.Manual;
            form6.Show();   
        }
    }
}
