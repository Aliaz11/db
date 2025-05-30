using System.Data.SqlClient;
using System.Data.SqlTypes;
using db;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
         
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;  
            this.Location = new Point(550, 200);            
            this.Size = new Size(1100,700 );                 

            this.FormBorderStyle = FormBorderStyle.FixedSingle; 
            this.MaximizeBox = false;                          

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
            byte[] imageBytes = Resource1.that; 
            using (var ms = new System.IO.MemoryStream(imageBytes))
            {
                var image = System.Drawing.Image.FromStream(ms);
                this.BackgroundImage = image;
            }

            this.BackgroundImageLayout = ImageLayout.Stretch;

        }
    }
}
