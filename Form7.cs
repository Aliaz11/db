using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;

namespace db
{
    public partial class Form7 : Form
    {
        string connection = Locator.GetConnectionString();
        public Form7()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
        }
        public byte[] getphoto()
        {
            MemoryStream stream = new MemoryStream();
            pictureBox1.Image.Save(stream, pictureBox1.Image.RawFormat);
            return stream.GetBuffer();
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            Book book = new Book
            {
                Name = textBox1.Text,
                author = textBox2.Text,
                price = textBox3.Text,
                quantity = numericUpDown1.Value,
                Date = dateTimePicker1.Value.ToString(),
                image = getphoto()

            };
 
            ;

            byte[] imageBytes = null;
            if (pictureBox1.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    imageBytes = ms.ToArray();
                }
            }
            DbCrudBook db = new DbCrudBook(connection);
            db.insert(book);

            textBox1.Text = textBox2.Text = textBox3.Text = "";
            numericUpDown1.Value = 0;
            pictureBox1.Image = null;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);



            }



        }


    }
}