namespace db
{
    public partial class Passchg : Form
    {
        string email;

        public Passchg(string email)
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
            this.email = email;
        }

        private void label1_Click(object? sender, EventArgs e)
        {

        }

        private void label2_Click(object? sender, EventArgs e)
        {

        }

        private void button1_Click(object? sender, EventArgs e)
        {
            PasswordUpdator passer = new PasswordUpdator(email, this);

            // updator reports its own validation failures; only a successful change navigates on.
            if (passer.updator(textBox1, textBox2))
            {
                Navigation.GoTo(this, new Form4());
            }
        }
    }
}
