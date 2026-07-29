namespace db
{

    public partial class Form5 : Form
    {

        string ids = "";
        string connection1 = Locator.GetConnectionString();
        byte[] images = Array.Empty<byte>();

        public Form5()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);

            dataGridView1.BackgroundColor = Color.White;
            ModernTheme.Apply(this);
            AppFeatures.EnableGridTools(this, dataGridView1, GridToolMode.Catalogue);
        }

        public Form5(string ids, byte[] images)
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);
            this.ids = ids ?? "";
            this.images = images ?? Array.Empty<byte>();

            dataGridView1.BackgroundColor = Color.White;
            ModernTheme.Apply(this);
            AppFeatures.EnableGridTools(this, dataGridView1, GridToolMode.Catalogue);
        }

        private void Form5_Load(object? sender, EventArgs e)
        {
            DataBaseCrud db1 = new DataBaseCrud(connection1);
            db1.selector(dataGridView1, this);

            AppFeatures.RefreshGridTools(this, dataGridView1, GridToolMode.Catalogue);
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form4());
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            // `ids` is a user id here, which is what DbCrudBook's parameter actually means.
            DbCrudBook tester = new DbCrudBook(ids);

            try
            {
                tester.inserter(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex.Message);
            }
        }

        private void pictureBox1_Click(object? sender, EventArgs e)
        {

        }

        private void button3_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form9(ids, images));
        }
    }
}
