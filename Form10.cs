using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using WinFormsApp3;
using db.Security;

namespace db
{
    public partial class Form10 : Form
    {
        decimal pricer = 0m;
        int bookcounter = 0;
        string connection = Locator.GetConnectionString();
        decimal full_income = 0m;

        public Form10()
        {
            InitializeComponent();
            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);

            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void Form10_Load(object? sender, EventArgs e)
        {
            if (Session.DenyIfNotAdmin())
            {
                // Deferred: closing a form from inside its own Load event is not safe.
                BeginInvoke(new Action(() => Navigation.GoTo(this, new Form4())));
                return;
            }

            // null (not "") means "no user seen yet", so no subtotal row is emitted before the first block.
            string? curruser = null;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                DataTable tb = new DataTable();
                tb.Columns.Add("user");
                tb.Columns.Add("name of the book");
                tb.Columns.Add("price of the book");

                const string querry = "SELECT iduser,bookname,price FROM saver1 ORDER BY iduser";

                conn.Open();

                using (SqlCommand cmd = new SqlCommand(querry, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string user = reader["iduser"].ToString() ?? "";

                        if (curruser != null && curruser != user)
                        {
                            AddSubtotalRow(tb);
                        }

                        curruser = user;

                        decimal price = ParsePrice(reader["price"]);

                        tb.Rows.Add(
                            user,
                            reader["bookname"].ToString(),
                            reader["price"].ToString());

                        pricer += price;
                        full_income += price;
                        bookcounter++;
                    }
                }

                // Closes the last user's block; skipped entirely when there were no rows at all.
                if (curruser != null)
                {
                    AddSubtotalRow(tb);
                }

                tb.Rows.Add("", "the total income is", full_income.ToString(CultureInfo.CurrentCulture));

                dataGridView1.DataSource = tb;
            }
        }

        private void AddSubtotalRow(DataTable tb)
        {
            tb.Rows.Add("total", "total books: " + bookcounter, "total payment: " + pricer);
            pricer = 0m;
            bookcounter = 0;
        }

        /// <summary>Prices are stored as text, so a decimal or an empty value must not throw.</summary>
        private static decimal ParsePrice(object? value)
        {
            string raw = (value?.ToString() ?? "").Trim();
            if (raw.Length == 0)
            {
                return 0m;
            }

            const NumberStyles styles = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;

            if (decimal.TryParse(raw, styles, CultureInfo.InvariantCulture, out decimal invariant))
            {
                return invariant;
            }

            if (decimal.TryParse(raw, styles, CultureInfo.CurrentCulture, out decimal current))
            {
                return current;
            }

            return 0m;
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            Navigation.GoTo(this, new Form3());
        }
    }
}
