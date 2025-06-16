using db;

namespace WinFormsApp3
{
    public partial class Form3 : Form
    {
        string connection = Locator.GetConnectionString();
        List<Book> Book = new List<Book>();

        public Form3()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
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

                    string firstname = string.IsNullOrWhiteSpace(textBox1.Text) ? listView1.SelectedItems[0].SubItems[1].Text : textBox1.Text;

                    string lastname = string.IsNullOrWhiteSpace(textBox2.Text) ? listView1.SelectedItems[0].SubItems[2].Text : textBox2.Text;

                    string phonenumber = string.IsNullOrWhiteSpace(textBox3.Text) ? listView1.SelectedItems[0].SubItems[3].Text : textBox3.Text;
                    string Email = string.IsNullOrWhiteSpace(textBox5.Text) ? listView1.SelectedItems[0].SubItems[5].Text : textBox5.Text;
                    string Username = string.IsNullOrWhiteSpace(textBox6.Text) ? listView1.SelectedItems[0].SubItems[8].Text : textBox6.Text;


                    string gender = radioButton1.Checked ? "Male" : radioButton2.Checked ? "Female" : listView1.SelectedItems[0].SubItems[6].Text;


                    string Birthdate = (dateTimePicker1.Value.ToString("yyyy-MM-dd") == listView1.SelectedItems[0].SubItems[4].Text) ? listView1.SelectedItems[0].SubItems[4].Text
                        : dateTimePicker1.Value.ToString("yyyy-MM-dd");


                    string password = string.IsNullOrWhiteSpace(textBox4.Text) ? listView1.SelectedItems[0].SubItems[7].Text : textBox4.Text;

                    DataBaseCrud dupdate = new DataBaseCrud(connection);
                    dupdate.update(listView1, firstname, lastname, phonenumber, gender, Birthdate, password, Email, Username);


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
            Form10 form10 = new Form10();
            form10.Location = this.Location;
            form10.Size = this.Size;
            form10.StartPosition = FormStartPosition.Manual;
            form10.Show();
        }


    }
}
