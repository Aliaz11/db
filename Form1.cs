using db;
using db.Security;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1300, 800);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void button4_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form2());
        }

        private void button3_Click_1(object? sender, EventArgs e)
        {
            // This button used to open Form3 — the admin user manager — directly, with no sign-in.
            // Administration is reached by signing in as an administrator; Form4 routes there.
            Navigation.GoTo(this, new Form4());
        }

        private void panel1_Paint(object? sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            // Returning to the main menu ends the session.
            Session.SignOut();

            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form4());
        }

        private void timer1_Tick(object? sender, EventArgs e)
        {

        }
    }
}
