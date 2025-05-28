using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
         
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;  // You control the exact position
            this.Location = new Point(550, 200);            // Set form location on screen (e.g., x=100, y=100)
            this.Size = new Size(1100,700 );                  // Set fixed size

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Prevent resizing
            this.MaximizeBox = false;                           // Disable maximize button

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Show();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Form2();
            form2.Show();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = Image.FromFile("C:\\Users\\ALI\\Downloads\\that.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

        }
    }
}
