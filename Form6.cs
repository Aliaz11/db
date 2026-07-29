using WinFormsApp3;
using db.Security;

namespace db
{

    public partial class Form6 : Form
    {
        /// <summary>Row selected in the grid; -1 until the user clicks one.</summary>
        int index = -1;

        string connection = Locator.GetConnectionString();

        public Form6()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // CellClick is already wired up in Form6.Designer.cs - subscribing here as well ran the
            // handler twice for every click.
            ModernTheme.Apply(this);
            AppFeatures.EnableGridTools(this, dataGridView1, GridToolMode.AdminBooks);
        }

        private void Form6_Load(object? sender, EventArgs e)
        {
            if (Session.DenyIfNotAdmin())
            {
                // Deferred: closing a form from inside its own Load event is not safe.
                BeginInvoke(new Action(() => Navigation.GoTo(this, new Form4())));
                return;
            }

            DataBaseCrud db = new DataBaseCrud(connection);
            db.selector(dataGridView1, this);

            AppFeatures.RefreshGridTools(this, dataGridView1, GridToolMode.AdminBooks);
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form3());
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form7());
        }

        private void button4_Click(object? sender, EventArgs e)
        {
            if (index < 0)
            {
                MessageBox.Show(
                    "Please select a book to delete by clicking on its row in the table.",
                    "No Book Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmResult = MessageBox.Show(
                "Are you sure you want to delete this book? This action cannot be undone.",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                DataBaseCrud db = new DataBaseCrud(connection);
                db.delete(dataGridView1, index);
                index = -1;
            }
        }

        private void dataGridView1_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            index = e.RowIndex;
            DataBaseCrud db1 = new DataBaseCrud(connection);
            db1.update(dataGridView1, index, textBox1, textBox2, textBox3, numericUpDown1, dateTimePicker1);
        }

        private void button2_Click_1(object? sender, EventArgs e)
        {
            string name = textBox1.Text;
            string author = textBox2.Text;
            string price = textBox3.Text;

            // Validate before touching the database, not after it has already been written to.
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(price))
            {
                MessageBox.Show("Please fill in all book details (Name, Author, Price).", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (index < 0)
            {
                MessageBox.Show("Please select a book row to update.", "No Book Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataBaseCrud db = new DataBaseCrud(connection);
            db.updateBase(dataGridView1, index, textBox1, textBox2, textBox3, numericUpDown1, dateTimePicker1);

            DataBaseCrud db5 = new DataBaseCrud(connection);
            db5.selector(dataGridView1, this);
        }
    }
}
