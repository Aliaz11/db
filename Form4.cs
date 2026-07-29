using Microsoft.Data.SqlClient;
using db.Data;
using db.Security;
using WinFormsApp3;

namespace db
{
    public partial class Form4 : Form
    {
        private readonly IAuthService authService = new AuthService();

        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            string userName = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Please enter both a username and a password.",
                    "Sign in",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                AuthenticatedUser? user = authService.Authenticate(userName, password);

                if (user == null)
                {
                    // Deliberately one message for both cases: telling the user which half was wrong
                    // lets an attacker enumerate valid usernames.
                    MessageBox.Show(
                        "The username or password is incorrect.",
                        "Sign in",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                Session.SignIn(user);

                if (user.IsAdmin)
                {
                    Navigation.GoTo(this, new Form3());
                }
                else
                {
                    Navigation.GoTo(this, new Form9(user.UserName, user.Photo));
                }
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "Could not reach the database. Please check your connection and try again.",
                    "Sign in",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form1());
        }

        private void Form4_Load(object? sender, EventArgs e)
        {
            byte[] imageBytes = Resource1.that;
            using (var ms = new System.IO.MemoryStream(imageBytes))
            {
                // Copied into a new Bitmap so the background survives the stream being disposed.
                this.BackgroundImage = new Bitmap(System.Drawing.Image.FromStream(ms));
            }

            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void label4_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form8());
        }
    }
}
