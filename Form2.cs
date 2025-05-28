using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WinFormsApp3
{
    public partial class Form2 : Form, Interface1
    {
        private string Firstname;
        private string Lastname;
        private string Phonenumber;
        private string Birhtdate;
        private string Email;

        public string lastname { get { return textBox1.Text; } set { Lastname = value; } }
        public string firstname { get { return textBox2.Text; } set { Firstname = value; } }
        public string phonenumber { get { return textBox3.Text; } set { Phonenumber = value; } }
        public string birthdate { get { return dateTimePicker1.Value.ToString(); } set { Birhtdate = value; } }
        public string email
        {
            get
            {
                // If textBox is null, return empty string
                if (textBox == null)
                    return "";
                return textBox.Text;
            }
            set
            {
                if (textBox != null)
                    textBox.Text = value;
                Email = value;
            }
        }
        private string Password;
        public string password { get {return textBox4.Text; }set {Password=value; } }
        private string PasswordR;
        public string passwordR { get { return textBox5.Text; } set { PasswordR = value; } }



        public void refresh() { }
        public Form2()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            insert();



        }
        List<String> list1 = new List<String>();

        public void insert()
        {
            try
            {

                string firstname = this.firstname;
                string lastname = this.lastname;
                string phonenumber = this.phonenumber;
                string email = this.email;
                string gender = "";
                string password = this.password;
                if (radioButton1.Checked)
                {
                    gender = "male";
                }
                else if (radioButton2.Checked)
                {
                    gender = "female";
                }
                string conection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
                string checkQuery = "SELECT COUNT(*) FROM Student WHERE phonenumber = @phonenumber";
                SqlConnection sqlconnection1 = new SqlConnection(conection_string);
                sqlconnection1.Open();
                SqlCommand command1 = new SqlCommand(checkQuery, sqlconnection1);
                command1.Parameters.AddWithValue("@phonenumber", phonenumber);
                int count = (int)command1.ExecuteScalar();
                sqlconnection1.Close();

                if (count > 0)
                {
                    MessageBox.Show("This phone number already exists in the database.");
                    textBox3.Text = "";
                    return;
                }
                list1.Add(phonenumber);
                string Birthdate = this.birthdate;
                if (!CommonFieldValidatorFunctions.RequiredFieldValid(firstname))
                {
                    MessageBox.Show("First name cannot be empty.");
                    return;
                }
                else if (!CommonFieldValidatorFunctions.RequiredFieldValid(firstname))
                {
                    MessageBox.Show("lastname cannot be empty.");
                    return;
                }
                if (textBox == null || String.IsNullOrEmpty(textBox.Text))
                {
                    if (!CommonFieldValidatorFunctions.RequiredFieldValid(firstname))
                    {
                        MessageBox.Show("phonenumber cannot be empty.");
                        return;
                    }
                }
                else if (!CommonFieldValidatorFunctions.StringFieldLengthValid(firstname, 2, 11))
                {
                    MessageBox.Show("firstname must be between 2 to 11 letters");
                    return;
                }
                else if (!CommonFieldValidatorFunctions.StringFieldLengthValid(lastname, 2, 11))
                {
                    MessageBox.Show("last name must be between 2 to 11 letters");
                    return;
                }
                if (textBox == null || String.IsNullOrEmpty(textBox.Text))
                {
                    if (!CommonFieldValidatorFunctions.StringFieldLengthValid(phonenumber, 10, 11))
                    {
                        MessageBox.Show("phone number must have 10 or 11 digits");
                        return;
                    }
                }
                if (!CommonFieldValidatorFunctions.PatternMatchValidDel(password,passwordR))

                {
                    MessageBox.Show("the passwords don't match");
                    return;

                }


                string query = $"INSERT INTO Student(firstname,lastname,phonenumber,Birthdate,Email,Gender)VALUES('{firstname}','{lastname}','{phonenumber}','{birthdate}','{email}','{gender}')";
                conection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
                using (SqlConnection sqlconnection2 = new SqlConnection(conection_string))
                {
                    sqlconnection2.Open();
                    MessageBox.Show("sucess");
                    SqlCommand command2 = new SqlCommand(query, sqlconnection2);
                    command2.ExecuteNonQuery();
                    sqlconnection2.Close();
                    textBox1.Text = textBox2.Text = textBox3.Text = "";

                }
                // Add subitems for other columns
                ListViewItem item = new ListViewItem(firstname);
                item.SubItems.Add(lastname);
                item.SubItems.Add(phonenumber);
                item.SubItems.Add(birthdate);
                if (string.IsNullOrEmpty(email))
                {
                    item.SubItems.Add("not provided");
                }
                else
                {
                    item.SubItems.Add(email);
                }
                item.SubItems.Add(gender);


                // Add the item to the ListView
                listView1.Items.Add(item);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = Image.FromFile("C:\\Users\\ALI\\Downloads\\that.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            listView1.View = View.Details;  // Show columns
            listView1.FullRowSelect = true; // Select entire row on click
            listView1.GridLines = true;     // Show grid lines for better readability

            // Add columns (adjust width and headers as you want)
            listView1.Columns.Add("First Name", 100);
            listView1.Columns.Add("Last Name", 100);
            listView1.Columns.Add("Phone Number", 120);
            listView1.Columns.Add("Birth Date", 100);
            listView1.Columns.Add("Email", 150);
            listView1.Columns.Add("Gender", 80);
            // Create a new ListViewItem with the first column (firstname)
            ListViewItem item = new ListViewItem(firstname);



        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 form = new Form1();
            form.Show();
        }
        public TextBox textBox;
        private void button3_Click(object sender, EventArgs e)
        {
            // Create a new TextBox
            textBox = new TextBox
            {
                Location = new System.Drawing.Point(700, 120), // Set position242, 63
                Size = new System.Drawing.Size(200, 30)     // Set size

            };
            Label label = new Label()
            {
                Location = new System.Drawing.Point(640, 120), // Set position242, 63
                Size = new System.Drawing.Size(200, 30)     // Set size


            };
            label.Text = "email";
            label.BackColor = Color.Transparent;
            // Add the TextBox to the form
            this.Controls.Add(textBox);
            this.Controls.Add(label);

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
