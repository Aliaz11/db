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
using System.Net.Http.Headers;


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


        public string firstname { get { return textBox2.Text; } set { Firstname = value; } }
        public string lastname { get { return textBox1.Text; } set { Lastname = value; } }

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
      

        public void insert()
        {Form2 f1= new Form2(); 
            f1.firstname = textBox1.Text;   
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
                    if (count > 0)
                    {
                        MessageBox.Show("This phone number already exists in the database.");
                        textBox3.Text = "";
                        return;
                    }

                }






                if (!CommonFieldValidatorFunctions.RequiredFieldValid(textBox1.Text))
                {

                    Label label_firstname = new Label()
                    {
                        Text = "firstname cannot be empty",
                        Name = "label_firstname",
                        BackColor = Color.Red,
                        Location = new Point(300, 80),
                        AutoSize = true,

                    };
                    this.Controls.Add(label_firstname);


                    return;
                }
                else
                {
                    Control[] a = this.Controls.Find("label_firstname", true);
                    foreach (Control i in a)
                    {
                        if (i.Text == "firstname cannot be empty")
                        {
                            this.Controls.Remove(i);
                        }
                    }
                }
                if (!CommonFieldValidatorFunctions.StringFieldLengthValid(textBox1.Text, 2, 11))
                {
                    Label label_firstname = new Label()
                    {
                        Text = "first name must be between 2 to 11 letters",
                        Name = "label_firstname",
                        BackColor = Color.Red,
                        Location = new Point(300, 80),
                        AutoSize = true,

                    };
                    this.Controls.Add(label_firstname);

                    return;
                }
                else
                {
                    Control[] a = this.Controls.Find("label_firstname", true);
                    foreach (Control i in a)
                    {
                        if (i.Text == "first name must be between 2 to 11 letters")
                        {
                            this.Controls.Remove(i);
                        }
                    }


                }





                if (!CommonFieldValidatorFunctions.RequiredFieldValid(textBox2.Text))
                {
                    Label label_lastname = new Label()
                    {
                        Name = "label_lastname",
                        Text = "lastname cannot be empty",
                        BackColor = Color.Red,
                        Location = new Point(300, 150),
                        AutoSize = true,

                    };
                    this.Controls.Add(label_lastname);
                    return;
                }
                else
                {
                    Control[] b = this.Controls.Find("label_lastname", true);
                    foreach (Control i in b)
                    {
                        if (i.Text == "lastname cannot be empty")
                        {
                            this.Controls.Remove(i);
                        }
                    }
                }
                if (!CommonFieldValidatorFunctions.StringFieldLengthValid(textBox2.Text, 2, 11))
                {
                    Label label_lastname = new Label()
                    {
                        Name = "label_lastname",
                        Text = "lastname must be between 2 to 11 letters",
                        BackColor = Color.Red,
                        Location = new Point(300, 150),
                        AutoSize = true,

                    };
                    this.Controls.Add(label_lastname);

                    return;
                }
                else
                {
                    Control[] b = this.Controls.Find("label_lastname", true);
                    foreach (Control i in b)
                    {
                        if (i.Text == "lastname must be between 2 to 11 letters")
                        {
                            this.Controls.Remove(i);
                        }
                    }

                }



                if (textBox == null && String.IsNullOrEmpty(email)&&string.IsNullOrEmpty(phonenumber))
                {
                    if (!CommonFieldValidatorFunctions.RequiredFieldValid(phonenumber) &&!CommonFieldValidatorFunctions.RequiredFieldValid(email))
                    {
                        Label label_phone = new Label()
                        {
                            Name = "label_phone",
                            Text = "phone number or email",
                            BackColor = Color.Red,
                            Location = new Point(300, 220),
                            AutoSize = true,

                        };
                        this.Controls.Add(label_phone);
                        return;
                    }
                   
                }
                else
                {
                    Control[] c = this.Controls.Find("label_phone", true);
                    foreach (Control i in c)
                    {
                        if (i.Text == "phone number or email")

                            this.Controls.Remove(i);

                    }
                }


                if (textBox == null || String.IsNullOrEmpty(email))
                {
                    if (!CommonFieldValidatorFunctions.StringFieldLengthValid(phonenumber, 10, 11))
                    {
                        Label label_phone = new Label()
                        {
                            Name = "label_phone",
                            Text = "phone number must be 10 or 11 digits",
                            BackColor = Color.Red,
                            Location = new Point(300, 220),
                            AutoSize = true,

                        };
                        this.Controls.Add(label_phone);
                        return;
                    }
                    else
                    {
                        Control[] d = this.Controls.Find("label_phone", true);
                        foreach (Control i in d)
                        {
                            if (i.Text == "phone number must be 10 or 11 digits")
                            {
                                this.Controls.Remove(i);
                            }
                        }
                    }


                }



                if (!radioButton1.Checked && !radioButton2.Checked)
                {
                    Label label_gender = new Label()
                    { Name = "label_gender",
                        Text = "gender must be chosen",
                        BackColor = Color.Red,
                        Location = new Point(300, 300),
                        AutoSize = true,

                    };
                    this.Controls.Add(label_gender);
                    return;
                }
                else if (radioButton1.Checked)
                {
                    gender = "male";
                    Control[] d = this.Controls.Find("label_gender", true);
                    foreach (Control i in d)
                    {
                        if (i.Text == ("gender must be chosen"))
                        {
                            this.Controls.Remove(i);
                        }
                    }
                }
                else if (radioButton2.Checked)
                {
                    gender = "female";
                    Control[] d = this.Controls.Find("label_gender", true);
                    foreach (Control i in d)
                    {
                        if (i.Text == ("gender must be chosen"))
                        {
                            this.Controls.Remove(i);
                        }
                    }
                }
                if (!CommonFieldValidatorFunctions.RequiredFieldValid(textBox4.Text))
                {
                    Label label_password = new Label()
                    {
                        Name = "password",
                        Text = "password must be entered",
                        BackColor = Color.Red,
                        Location = new Point(300, 400),
                        AutoSize = true,

                    };
                    this.Controls.Add(label_password);
                    return;
                }
                else
                {
                    Control[] d = this.Controls.Find("password", true);
                    foreach (Control i in d)
                    {
                        if (i.Text == "password must be entered")
                        {
                            this.Controls.Remove(i);
                        }
                    }

                }
                if ((!CommonFieldValidatorFunctions.FieldPatternValid(password, regex.Strong_Password_RegEx_Pattern)))
                {
                    Label label_password1 = new Label()
                    {
                        Name = "password1",
                        Text = "the password must have one capital letter one small letter one special character",
                        BackColor = Color.Red,
                        Location = new Point(300, 400),
                        AutoSize = true,

                    };
                    this.Controls.Add(label_password1);

                    return;

                }
                else
                {
                    Control[] d = this.Controls.Find("password1", true);
                    foreach (Control i in d)
                    {
                        if (i.Text == "the password must have one capital letter one small letter one special character")
                        {
                            this.Controls.Remove(i);
                        }
                    }
                }


                if (!CommonFieldValidatorFunctions.PatternMatchValidDel(password, passwordR))

                {
                    Label label_password = new Label()
                    {
                        Name = "password",
                        Text = "password dont match",
                        BackColor = Color.Red,
                        Location = new Point(300, 400),
                        AutoSize = true,

                    };
                    this.Controls.Add(label_password);

                    return;

                }
                else
                {
                    Control[] d = this.Controls.Find("password", true);
                    foreach (Control i in d)
                    {
                        if (i.Text == "password dont match")
                        {
                            this.Controls.Remove(i);
                        }
                    }
                }





                    Label sucess = new Label()
                    {
                        Name = "sucess",
                        Text = "sucess",
                        BackColor = Color.Green,
                        Location = new Point(300, 530),
                        AutoSize = true,
                    };
                this.Controls.Add(sucess);  


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
            listView1.View = View.Details;

            listView1.Columns.Add("First Name", 100);
            listView1.Columns.Add("Last Name", 100);
            listView1.Columns.Add("Phone Number", 120);
            listView1.Columns.Add("Birth Date", 100);
            listView1.Columns.Add("Email", 150);
            listView1.Columns.Add("Gender", 80);
            this.Controls.Add(listView1);



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
                Location = new System.Drawing.Point(720, 190),
                Size = new System.Drawing.Size(200, 30)

            };
            Label label = new Label()
            {
                Location = new System.Drawing.Point(670, 190),
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




