using WinFormsApp3;

namespace db
{

    public partial class Form6 : Form
    {
        int index;
        string id;

        string connection = Locator.GetConnectionString();
        public Form6(List<Book> books)
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.CellClick += dataGridView1_CellClick;



        }


        private void Form6_Load(object sender, EventArgs e)
        {
            




            DataBaseCrud db = new DataBaseCrud(connection);
            db.selector(dataGridView1, this);
      

            dataGridView1.RowTemplate.Height = 100;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = 100;
            }



        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Form3 form1 = new Form3();
            form1.Location = this.Location;
            form1.Size = this.Size;
            form1.StartPosition = FormStartPosition.Manual;
            form1.Show();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {


            /*   if (string.IsNullOrEmpty(id))
               {
                   MessageBox.Show("Please select a book to delete by clicking on its row in the table.", "No Book Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   return;
               }*/

            DialogResult confirmResult = MessageBox.Show(
                "Are you sure you want to delete this book? This action cannot be undone.",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {

                DataBaseCrud db = new DataBaseCrud(connection);
                db.delete(dataGridView1, index);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0)

            { return; }


            index = e.RowIndex;
            DataBaseCrud db1 = new DataBaseCrud(connection);
            db1.update(dataGridView1, index, textBox1, textBox2, textBox3,numericUpDown1,dateTimePicker1);

 



   
        }





        private void button2_Click_1(object sender, EventArgs e)
        {

            DataBaseCrud db = new DataBaseCrud(connection);
            db.updateBase(dataGridView1, index, textBox1, textBox2, textBox3, numericUpDown1, dateTimePicker1);
    

            string name = textBox1.Text;
            string author = textBox2.Text;
            string price = textBox3.Text;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(price))
            {
                MessageBox.Show("Please fill in all book details (Name, Author, Price).", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataBaseCrud db5 = new DataBaseCrud(connection);
            db5.selector(dataGridView1, this);




        }


    }
}


