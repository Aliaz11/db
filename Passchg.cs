using Microsoft.Data.SqlClient;
using WinFormsApp3;

namespace db
{
    public partial class Passchg : Form
    {
        string email;
        string connection = Locator.GetConnectionString();
        string password;
        public Passchg(string email)
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
            this.email = email;



        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            PasswordUpdator passer = new PasswordUpdator(email, this);
            passer.updator(textBox1,textBox2);
        }
    }
}
