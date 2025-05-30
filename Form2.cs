using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using db;


namespace WinFormsApp3
{
    public partial class Form2 : Form, Interface1
    {
        private string Firstname;
        private string Lastname;
        private string Phonenumber;
        private string Birhtdate;
        private string Email;
        private string Password;
        private string PasswordR;
        public string lastname { get { return textBox1.Text; } set { Lastname = value; } }
        public string firstname { get { return textBox2.Text; } set { Firstname = value; } }
        public string phonenumber { get { return textBox3.Text; } set { Phonenumber = value; } }
        public string birthdate { get { return dateTimePicker1.Value.ToString(); } set { Birhtdate = value; } }
        public string email
        {
            get { if (textBox == null) return ""; return textBox.Text; }
            set
            {
                if (textBox != null) textBox.Text = value; Email = value;
            }
        }
        public string password { get { return textBox4.Text; } set { Password = value; } }
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
                string Birthdate = this.birthdate;

                string conection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
                string checkQuery = "SELECT COUNT(*) FROM Student WHERE phonenumber = @phonenumber";
                using (SqlConnection sqlConnection1 = new SqlConnection(conection_string))
                {
                    sqlConnection1.Open();
                    SqlCommand command1 = new SqlCommand(checkQuery, sqlConnection1);
                    command1.Parameters.AddWithValue("@phonenumber", phonenumber);
                    int count = (int)command1.ExecuteScalar();
                    sqlConnection1.Close();

                    if (radioButton1.Checked)
                    {
                        gender = "male";
                    }
                    else if (radioButton2.Checked)
                    {
                        gender = "female";
                    }


                    if (count > 0)
                    {
                        MessageBox.Show("This phone number already exists in the database.");
                        textBox3.Text = "";
                        return;
                    }
                    list1.Add(phonenumber);

                    if (!CommonFieldValidatorFunctions.RequiredFieldValid(firstname))
                    {
                        Label label_1 = new Label()
                        {
                            BackColor = Color.Transparent,
                            ForeColor = Color.Red,
                            Text = "**the Firstname cannot be empty",
                            Location = new System.Drawing.Point(274, 80),

                            Size = new System.Drawing.Size(300, 30)

                        };
                        this.Controls.Add(label_1);

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
                    if (!CommonFieldValidatorFunctions.FieldPatternValid(password, regex.Strong_Password_RegEx_Pattern))
                    {
                        MessageBox.Show("the pass word isnt strong enough");
                        return;
                    }
                    if (!CommonFieldValidatorFunctions.PatternMatchValidDel(password, passwordR))

                    {
                        MessageBox.Show("the passwords don't match");
                        return;

                    }
                }


                string query = $"INSERT INTO Student(firstname,lastname,phonenumber,Birthdate,Email,Gender,Password)VALUES(@firstname,@lastname,@phonenumber,@birthdate,@email,@gender,@password)";
                conection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Documents\\c#\\db\\Stu.mdf;Integrated Security=True";
                using (SqlConnection sqlconnection2 = new SqlConnection(conection_string))
                {
                    sqlconnection2.Open();

                    SqlCommand command2 = new SqlCommand(query, sqlconnection2);
                    command2.Parameters.AddWithValue("@firstname", firstname);
                    command2.Parameters.AddWithValue("@lastname", lastname);
                    command2.Parameters.AddWithValue("@phonenumber", phonenumber);
                    command2.Parameters.AddWithValue("@birthdate", Birthdate);
                    command2.Parameters.AddWithValue("@email", email);
                    command2.Parameters.AddWithValue("@gender", gender);
                    command2.Parameters.AddWithValue("@password", password);
                    command2.ExecuteNonQuery();
                    sqlconnection2.Close();
                    textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = "";

                }

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
                if (string.IsNullOrEmpty(phonenumber))
                {
                    item.SubItems.Add("not provided");
                }
                else
                {
                    item.SubItems.Add(phonenumber);
                }
                item.SubItems.Add(gender);

                listView1.Items.Add(item);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            ;
            {
                byte[] imageBytes = Resource1.that;
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    var image = System.Drawing.Image.FromStream(ms);
                    this.BackgroundImage = image;
                }

                this.BackgroundImageLayout = ImageLayout.Stretch;

            }

            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.OwnerDraw = true;

            listView1.Columns.Add("First Name", 100);
            listView1.Columns.Add("Last Name", 100);
            listView1.Columns.Add("Phone Number", 120);
            listView1.Columns.Add("Birth Date", 100);
            listView1.Columns.Add("Email", 150);
            listView1.Columns.Add("Gender", 80);

            ListViewItem item = new ListViewItem(firstname);



            textBox4.UseSystemPasswordChar = true;
            textBox5.UseSystemPasswordChar=true;

            checkBox1.Text = "View";



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

            textBox = new TextBox
            {
                Location = new System.Drawing.Point(700, 140),
                Size = new System.Drawing.Size(200, 30)

            };
            Label label = new Label()
            {
                Location = new System.Drawing.Point(700, 140),
                Size = new System.Drawing.Size(200, 30)


            };
            label.Text = "email";
            label.BackColor = Color.Transparent;
            this.Controls.Add(textBox);
            this.Controls.Add(label);


        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                PictureBox p1 = new PictureBox
                {
                    Image = Image.FromFile(openFileDialog1.FileName),
                    Location = new Point(600, 330),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(100, 100),
                };


                this.Controls.Add(p1);

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }


        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)

            {
                textBox4.UseSystemPasswordChar = false;
                textBox5.UseSystemPasswordChar = false;
                var checkbox = (CheckBox)sender;
                checkbox.Text = "View";
            }
            else
            {
                textBox4.UseSystemPasswordChar = true;
                textBox5.UseSystemPasswordChar = true;
                var checkbox = (CheckBox)sender;
                checkbox.Text = "Hide";
            }

        }
    }
    }




