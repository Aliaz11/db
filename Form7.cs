namespace db
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
        }

        /// <summary>Cover image bytes, or an empty array when no image was chosen.</summary>
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

        private void button1_Click(object? sender, EventArgs e)
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

            // DbCrudBook's constructor parameter is a *user* id, which this screen has none of - it adds
            // to the shared Books catalogue. It used to be handed the connection string by mistake.
            DbCrudBook db = new DbCrudBook(string.Empty);
            db.insert(book);

            textBox1.Text = textBox2.Text = textBox3.Text = "";
            numericUpDown1.Value = 0;
            pictureBox1.Image = null;
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            }
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form6());
        }
    }
}
