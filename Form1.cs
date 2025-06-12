
using System.Data.SqlTypes;
using System.Diagnostics.Metrics;
using Microsoft.Data.SqlClient;
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
            this.Size = new Size(1300, 800);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
           

           

        }



        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Form2();
            form2.Location = this.Location;
            form2.Size = this.Size;
            form2.StartPosition = FormStartPosition.Manual;
            form2.Show();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form3 form3 = new Form3();
            form3.Location = this.Location;
            form3.Size = this.Size;
            form3.StartPosition = FormStartPosition.Manual;

            form3.StartPosition = FormStartPosition.Manual;
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




            BackPhoto.BackSet(this);

        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Location = this.Location;
            form4.Size = this.Size;
            form4.StartPosition = FormStartPosition.Manual;
            form4.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
