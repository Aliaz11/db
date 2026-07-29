using db;
using db.Security;

namespace WinFormsApp3
{
    public partial class Form3 : Form
    {
        string connection = Locator.GetConnectionString();

        public Form3()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
            ListViewCre listViewCre = new ListViewCre();
            listViewCre.ListViewCre1(listView1, this);
        }

        private void label2_Click(object? sender, EventArgs e)
        {

        }

        private void Form3_Load(object? sender, EventArgs e)
        {
            if (Session.DenyIfNotAdmin())
            {
                // Deferred: closing a form from inside its own Load event is not safe.
                BeginInvoke(new Action(() => Navigation.GoTo(this, new Form4())));
                return;
            }

            DataBaseCrud dbc = new DataBaseCrud(connection);
            dbc.selector(listView1);
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                try
                {
                    string firstname = string.IsNullOrWhiteSpace(textBox1.Text) ? listView1.SelectedItems[0].SubItems[1].Text : textBox1.Text;

                    string lastname = string.IsNullOrWhiteSpace(textBox2.Text) ? listView1.SelectedItems[0].SubItems[2].Text : textBox2.Text;

                    string phonenumber = string.IsNullOrWhiteSpace(textBox3.Text) ? listView1.SelectedItems[0].SubItems[3].Text : textBox3.Text;
                    string Email = string.IsNullOrWhiteSpace(textBox5.Text) ? listView1.SelectedItems[0].SubItems[5].Text : textBox5.Text;
                    string Username = string.IsNullOrWhiteSpace(textBox6.Text) ? listView1.SelectedItems[0].SubItems[8].Text : textBox6.Text;

                    string gender = radioButton1.Checked ? "Male" : radioButton2.Checked ? "Female" : listView1.SelectedItems[0].SubItems[6].Text;

                    string Birthdate = (dateTimePicker1.Value.ToString("yyyy-MM-dd") == listView1.SelectedItems[0].SubItems[4].Text) ? listView1.SelectedItems[0].SubItems[4].Text
                        : dateTimePicker1.Value.ToString("yyyy-MM-dd");

                    // The list view only ever shows a mask for the password, so it can never be used as a
                    // value. An empty textbox means "leave the stored password unchanged".
                    string password = textBox4.Text;

                    DataBaseCrud dupdate = new DataBaseCrud(connection);
                    dupdate.update(listView1, firstname, lastname, phonenumber, gender, Birthdate, password, Email, Username);

                    textBox4.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("select a row ");
                return;
            }
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form1());
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                DataBaseCrud b1 = new DataBaseCrud(connection);

                b1.delete(listView1);
            }
            else
            {
                MessageBox.Show("Please select a row to remove");
            }
        }

        private void listView1_SelectedIndexChanged(object? sender, EventArgs e)
        {

        }

        private void button5_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form6());
        }

        private void button6_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form10());
        }
    }
}
