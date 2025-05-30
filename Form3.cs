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

namespace WinFormsApp3
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            listView1.View = View.List;
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;

            listView1.Columns.Add("First Name", 100);
            listView1.Columns.Add("Last Name", 100);
            listView1.Columns.Add("Phone Number", 120);
            listView1.Columns.Add("Birth Date", 100);
            listView1.Columns.Add("Email", 150);
            listView1.Columns.Add("Gender", 80);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        List<string> list = new List<string>();
        void refresh()
        {

            try
            {
                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connection_string);
                sqlConnection.Open();
                string query = "SELECT * FROM Student";
                SqlCommand command = new SqlCommand(query, sqlConnection);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            {
                byte[] imageBytes = Resource1.that;
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    var image = System.Drawing.Image.FromStream(ms);
                    this.BackgroundImage = image;
                }

                this.BackgroundImageLayout = ImageLayout.Stretch;

            }
            string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();
            string query = "SELECT * FROM Student";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            listView1.Items.Clear();

            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader["firstname"].ToString());
                item.SubItems.Add(reader["lastname"].ToString());
                item.SubItems.Add(reader["phonenumber"].ToString());
                item.SubItems.Add(reader["birthdate"].ToString());
                item.SubItems.Add(reader["email"].ToString());
                item.SubItems.Add(reader["gender"].ToString());

                listView1.Items.Add(item);
            }


            refresh();




        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                string firstname = textBox1.Text;
                string lastname = textBox2.Text;
                string phonenumber = textBox3.Text;





                string query = $"UPDATE Student SET firstname='{firstname}',lastname='{lastname}',phonenumber='{phonenumber}'WHERE id=''";
                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connection_string);
                sqlConnection.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection);
                textBox1.Text = textBox2.Text = textBox3.Text = "";
                if (command.ExecuteNonQuery() > 0)
                {
                    refresh();
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
                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connection_string);
                sqlConnection.Open();

                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    string phonenumber = item.SubItems[2].Text;


                    string query = "DELETE FROM Student WHERE phonenumber = @phonenumber";
                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@phonenumber", phonenumber);


                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        listView1.Items.Remove(item);
                    }
                    else
                    {
                        MessageBox.Show($"Could not delete record with phone number {phonenumber}");
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
    }
}
