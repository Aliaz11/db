namespace db
{
    public partial class Form8 : Form
    {
        Emailverifycs em1 = new Emailverifycs();

        int random;
        public Form8()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            em1.EmailSender(textBox1.Text);
            MessageBox.Show("try again in 1 minutes");
            button1.Enabled = false;
            for (int i = 60; i >= 0; i--)
            {
                label3.Text = i.ToString();
                await Task.Delay(1000);

            }

            button1.Enabled = true;




        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Passchg pass1 = new Passchg(textBox1.Text);
            em1.adapt(textBox2, pass1, this);
        }

        private void Form8_Load(object sender, EventArgs e)
        {

        }
    }
}
