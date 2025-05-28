using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WinFormsApp3
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            this.BackColor = System.Drawing.Color.Black;
        }
        void refresh()
        {
            try
            {
                comboBox1.Items.Clear();
                string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
                SqlConnection sqlConnection = new SqlConnection(connection_string);
                sqlConnection.Open();
                string query = "SELECT * FROM Student";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    String option = reader["Id"].ToString() + "-" + reader["firstname"].ToString();
                    comboBox1.Items.Add(option);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Form4_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = Image.FromFile("C:\\Users\\ALI\\Downloads\\that.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = comboBox1.SelectedItem.ToString();
            string sp = name.Split('-')[0];
            string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();
            string query = $"DELETE FROM Student WHERE id={sp}";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.ExecuteNonQuery();

            refresh();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 form = new Form1();
            form.Show();
        }
    }
}
