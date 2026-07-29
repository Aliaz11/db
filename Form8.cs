using System.Net.Mail;

namespace db
{
    public partial class Form8 : Form
    {
        Emailverifycs em1 = new Emailverifycs();

        public Form8()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
        }

        private async void button1_Click(object? sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();

            if (!LooksLikeEmail(email))
            {
                MessageBox.Show(
                    "Please enter a valid email address.",
                    "Password reset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            button1.Enabled = false;

            try
            {
                if (!em1.SendCode(email))
                {
                    MessageBox.Show(
                        "The verification email could not be sent. Please check the address and try again.",
                        "Password reset",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("A verification code has been sent. You can request another one in 1 minute.");

                for (int i = 60; i >= 0; i--)
                {
                    label3.Text = i.ToString();
                    await Task.Delay(1000);
                }
            }
            finally
            {
                // The button has to come back even when sending threw, otherwise the screen is dead.
                button1.Enabled = true;
            }
        }

        /// <summary>Cheap sanity check so an obviously wrong address never reaches the mail server.</summary>
        private static bool LooksLikeEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            try
            {
                MailAddress address = new MailAddress(value);
                return string.Equals(address.Address, value, StringComparison.Ordinal)
                    && address.Host.Contains('.', StringComparison.Ordinal);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void label2_Click(object? sender, EventArgs e)
        {

        }

        private void button2_Click(object? sender, EventArgs e)
        {
            Passchg pass1 = new Passchg(textBox1.Text.Trim());
            em1.adapt(textBox2, pass1, this);
        }

        private void Form8_Load(object? sender, EventArgs e)
        {

        }

        private void button3_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form4());
        }
    }
}
