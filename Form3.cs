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
        string connection = Locator.GetConnectionString();
        List<Book> Book = new List<Book>();

        public Form3()
        {
            InitializeComponent();
            BackPhoto.BackSet(this);
            ListViewCre listViewCre = new ListViewCre();
            listViewCre.ListViewCre1(listView1, this);


        }
        private void label2_Click(object sender, EventArgs e)
        {

        }


        /*public string Counter(string query)
         {

             using (SqlConnection sqlConnection = new SqlConnection(connection))
             {

                 sqlConnection.Open();
                 SqlCommand command = new SqlCommand(query, sqlConnection);
                 string count = command.ExecuteScalar().ToString();
                 SqlDataReader reader = command.ExecuteReader();
                 return count;
                 sqlConnection.Close();
             }
            public string male_counter()
         {
             string con = Counter("SELECT COUNT(*) FROM Stu1 WHERE Gender='male'");
             return con;
         }
         public string female_counter()
         {
             string con = Counter("SELECT COUNT(*) FROM Stu1 WHERE Gender='female'");
             return con;
         }
        */







        private void Form3_Load(object sender, EventArgs e)
        {
        
            DataBaseCrud dbc = new DataBaseCrud(connection);
            dbc.selector(listView1);

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
              

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
                    DataBaseCrud dupdate = new DataBaseCrud(connection);
                    dupdate.update(listView1, firstname, lastname, phonenumber, gender, Birthdate, password);


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("select a row ");
                return;

            }
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
                DataBaseCrud b1 = new DataBaseCrud(connection);

                b1.delete(listView1);
            }

            else
            {
                MessageBox.Show("Please select a row to remove");
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6(Book);
            form6.Location = this.Location;
            form6.Size = this.Size;
            form6.StartPosition = FormStartPosition.Manual;
            this.Close();
            form6.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form10 form10=new Form10();
            form10.Location = this.Location;
            form10.Size = this.Size;
            form10.StartPosition = FormStartPosition.Manual;
            form10.Show();
        }

 
    }
}
