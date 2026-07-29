using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using WinFormsApp3;
using db.Security;

namespace db
{
    public partial class Form10 : Form
    {
        private readonly string connection = Locator.GetConnectionString();
        private readonly SalesDashboard salesDashboard;

        public Form10()
        {
            InitializeComponent();
            salesDashboard = new SalesDashboard
            {
                Name = "modernSalesDashboard",
                TabStop = true
            };
            Controls.Add(salesDashboard);

            BackPhoto bc = new BackPhoto();

            bc.BackSet(this);

            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ModernTheme.Apply(this);
            AppFeatures.EnableGridTools(this, dataGridView1, GridToolMode.Payments);
        }

        private void Form10_Load(object? sender, EventArgs e)
        {
            if (Session.DenyIfNotAdmin())
            {
                // Deferred: closing a form from inside its own Load event is not safe.
                BeginInvoke(new Action(() => Navigation.GoTo(this, new Form4())));
                return;
            }

            using (SqlConnection conn = new SqlConnection(connection))
            {
                DataTable tb = new DataTable();
                tb.Columns.Add("Customer", typeof(string));
                tb.Columns.Add("Book", typeof(string));
                tb.Columns.Add("Author", typeof(string));
                tb.Columns.Add("Price", typeof(decimal));
                List<SaleRecord> sales = new List<SaleRecord>();

                const string query = """
                    SELECT iduser, bookname, author, price
                    FROM dbo.saver1
                    ORDER BY iduser, bookname
                    """;

                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string user = reader["iduser"]?.ToString()?.Trim() ?? "";
                        string book = reader["bookname"]?.ToString()?.Trim() ?? "";
                        string author = reader["author"]?.ToString()?.Trim() ?? "";
                        decimal price = ParsePrice(reader["price"]);

                        tb.Rows.Add(user, book, author, price);
                        sales.Add(new SaleRecord(user, book, author, price));
                    }
                }

                dataGridView1.DataSource = tb;
                if (dataGridView1.Columns["Price"] is { } priceColumn)
                {
                    priceColumn.DefaultCellStyle.Format = "C2";
                    priceColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                salesDashboard.SetSales(sales);
            }

            AppFeatures.RefreshGridTools(this, dataGridView1, GridToolMode.Payments);
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
