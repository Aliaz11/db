namespace db
{

    public partial class Form5 : Form
    {

        string ids;
        string connection1 = Locator.GetConnectionString();
        byte[] images;

        public Form5()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);

        }
        public Form5(string ids, byte[] images)
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
            this.ids = ids;
            this.images=images;

            dataGridView1.BackgroundColor = Color.White;

        }


        private void Form5_Load(object sender, EventArgs e)
        {

            DataBaseCrud db1 = new DataBaseCrud(connection1);
            db1.selector(dataGridView1, this);
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = 100;
            }

        }



        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            this.Close();
            form4.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DbCrudBook tester = new DbCrudBook(ids);
            tester.inserter(dataGridView1);


            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            Form9 form9 = new Form9(ids,images);
            form9.ShowDialog();
        }
    }
}

