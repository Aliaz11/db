
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
            this.Size = new Size(1100, 700);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

        }



        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Form2();
            form2.Location = this.Location;
            form2.Size=this.Size;
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
            label2.Text = male_counter();
            label4.Text = female_counter();   

        }
        public string male_counter()
        {
            string con=Counter("SELECT COUNT(*) FROM Stu1 WHERE Gender='male'");
            return con;
        }
        public string female_counter()
        {
            string con = Counter("SELECT COUNT(*) FROM Stu1 WHERE Gender='female'");
            return con;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Location=this.Location;
            form4.Size = this.Size;
            form4.StartPosition = FormStartPosition.Manual;
            form4.Show();
            
        }
        public string Counter(string query)
        {
            string connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ALI\\Pictures\\second\\Stu2.mdf;Integrated Security=True";
            using (SqlConnection sqlConnection = new SqlConnection(connection_string))
            {
                
                sqlConnection.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection);
                string count=command.ExecuteScalar().ToString();
                SqlDataReader reader = command.ExecuteReader();
                return count;
                sqlConnection.Close();
            }

      

        }
    }
}
