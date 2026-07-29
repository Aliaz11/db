using db;

namespace WinFormsApp3
{
    public partial class Form2 : Form
    {

        string connection = Locator.GetConnectionString();
        private LabelValidator _validator = new LabelValidator();

        /// <summary>
        /// Optional email box, created once and only added to the form when the user asks for it
        /// with button3. It is never null so it can always be handed to <see cref="LabelValidator.selectoring"/>.
        /// </summary>
        public TextBox textBox = new TextBox
        {
            Name = "emailTextBox",
            Location = new System.Drawing.Point(720, 190),
            Size = new System.Drawing.Size(200, 30)
        };

        private readonly Label emailLabel = new Label
        {
            Name = "emailLabel",
            Location = new System.Drawing.Point(670, 190),
            Size = new System.Drawing.Size(200, 30),
            Text = "email",
            BackColor = Color.Transparent
        };

        public Form2()
        {
            InitializeComponent();
            Controls.Add(textBox);
            Controls.Add(emailLabel);
            ModernTheme.Apply(this);
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            var allLabelsList = new List<string>
            {
                "label_firstname",
                "label_lastname",
                "label_phone",
                "label_gender",
                "label_password",
                "label_password1"
            };

            var user = new IUser
            {
                FirstName = textBox1.Text,
                LastName = textBox2.Text.Trim(),
                PhoneNumber = textBox3.Text.Trim(),
                BirthDate = dateTimePicker1.Value,
                Email = textBox.Text.Trim(),
                Password = textBox4.Text,
                PasswordR = textBox5.Text,
                UserName = textBox6.Text.Trim(),
                Gender = radioButton1.Checked ? "male" : radioButton2.Checked ? "female" : "",
                Photo = getphoto()
            };

            foreach (var name in allLabelsList)
            {
                LabelValidator.RemoveValidationLabel(name, this);
            }

            // selectoring returns void; when it finds a duplicate it reports it and blanks the offending
            // textbox. Comparing the boxes before and after is the only way to see that from here, and it
            // has to gate the insert - otherwise the duplicate row is written anyway.
            string emailBefore = textBox.Text;
            string userNameBefore = textBox6.Text;

            LabelValidator uy = new LabelValidator();
            uy.selectoring(user.Email, user.UserName, textBox, textBox6);

            if (textBox.Text != emailBefore || textBox6.Text != userNameBefore)
            {
                return;
            }

            var firstError = _validator.Validate(user).FirstOrDefault();
            if (firstError != null)
            {
                LabelValidator.ShowValidationLabel(firstError.FieldName, firstError.Message, firstError.Position, this);

                return;
            }

            DataBaseCrud db = new DataBaseCrud(connection);
            db.insert(user.FirstName, user.LastName, user.PhoneNumber, user.BirthDate.ToString(), user.Email, user.Gender, user.Password, user.UserName, user.Photo);

            ListViewItem item = new ListViewItem(user.FirstName);
            item.SubItems.Add(user.LastName);
            item.SubItems.Add(user.PhoneNumber);
            item.SubItems.Add(user.BirthDate.ToString());

            if (string.IsNullOrEmpty(user.Email))
            {
                item.SubItems.Add("not provided");
            }
            else
            {
                item.SubItems.Add(user.Email);
            }
            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                item.SubItems.Add("not provided");
            }
            else
            {
                item.SubItems.Add(user.PhoneNumber);
            }
            item.SubItems.Add(user.Gender);

            listView1.Items.Add(item);
        }

        private void Form2_Load(object? sender, EventArgs e)
        {
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);

            ListViewCre listViewCre = new ListViewCre();
            listViewCre.ListViewCre1(listView1, this);

            textBox4.UseSystemPasswordChar = true;
            textBox5.UseSystemPasswordChar = true;

            checkBox1.Text = "View";
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form1());
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            // The controls are created once; clicking again must not stack another pair on the form.
            if (!this.Controls.Contains(textBox))
            {
                this.Controls.Add(textBox);
                this.Controls.Add(emailLabel);
            }

            textBox.Focus();
        }

        private void button4_Click(object? sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            }
        }

        /// <summary>Profile photo bytes, or an empty array when no photo was chosen.</summary>
        public byte[] getphoto()
        {
            if (pictureBox1.Image == null)
            {
                return Array.Empty<byte>();
            }

            using (MemoryStream stream = new MemoryStream())
            {
                pictureBox1.Image.Save(stream, pictureBox1.Image.RawFormat);
                return stream.ToArray();
            }
        }

        private void checkBox1_CheckedChanged_1(object? sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox4.UseSystemPasswordChar = false;
                textBox5.UseSystemPasswordChar = false;
                checkBox1.Text = "View";
            }
            else
            {
                textBox4.UseSystemPasswordChar = true;
                textBox5.UseSystemPasswordChar = true;
                checkBox1.Text = "Hide";
            }
        }
    }
}
