using db;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace WinFormsApp3
{
    public partial class Form2 : Form
    {

        string connection = Locator.GetConnectionString();
        private LabelValidator _validator = new LabelValidator();


        public void refresh() { }
        public Form2()
        {
            InitializeComponent();
            DataBaseCrud db_in = new DataBaseCrud(connection);
     
        }



        private void button1_Click(object sender, EventArgs e)
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
                Email = textBox?.Text.Trim() ?? "",
                Password = textBox4.Text,
                PasswordR = textBox5.Text,
                UserName = textBox6.Text.Trim(),
                Gender = radioButton1.Checked ? "male" : radioButton2.Checked ? "female" : "",
                Photo = pictureBox1.Image != null ? getphoto() : Array.Empty<byte>()

            };
            foreach (var name in allLabelsList)
            {
                LabelValidator.RemoveValidationLabel(name, this);
            }
            LabelValidator uy = new LabelValidator();
            uy.selectoring(user.Email,user.UserName,textBox,textBox6);




            var firstError = _validator.Validate(user).FirstOrDefault();
            if (firstError != null)
            {
                LabelValidator.ShowValidationLabel(firstError.FieldName, firstError.Message, firstError.Position,this);

                return;
            }

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
            DataBaseCrud db = new DataBaseCrud(connection);
            db.insert(user.FirstName, user.LastName, user.PhoneNumber, user.BirthDate.ToString(), user.Email, user.Gender, user.Password, user.UserName, getphoto());



        }



        

        private void Form2_Load(object sender, EventArgs e)
        {


            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);

            ListViewCre listViewCre = new ListViewCre();
            listViewCre.ListViewCre1(listView1, this);






            textBox4.UseSystemPasswordChar = true;
            textBox5.UseSystemPasswordChar = true;

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
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);



            }
        }


        

        public byte[] getphoto()
        {
            MemoryStream stream = new MemoryStream();
            pictureBox1.Image.Save(stream, pictureBox1.Image.RawFormat);
            return stream.GetBuffer();
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




