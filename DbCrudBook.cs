using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;

namespace db
{
    public class DbCrudBook
    {
        int price1 = 0;
        string connection1=Locator.GetConnectionString();
        string id = "";
        public DbCrudBook(string id) { 
        this.id = id;
        
        }
        public void inserter(DataGridView dataGridView1)
        {
            var selectedBooks = new List<Book>();
          

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {


                bool IsChecked = Convert.ToBoolean(row.Cells["chk"].Value);
                if (IsChecked)
                {

                    string bookName = row.Cells["name"].Value?.ToString() ?? "";
                    string author = row.Cells["author"].Value?.ToString() ?? "";
                    string price = row.Cells["price"].Value?.ToString() ?? "";
                    price1 += Convert.ToInt32(price);

                    selectedBooks.Add(new Book { Name =bookName,author=author,price=price});
                }
            }
            MessageBox.Show("the price to pay: " + price1);

            if (selectedBooks.Count == 0)
            {
                MessageBox.Show("No books selected.");
                return;
            }



            using (SqlConnection conn = new SqlConnection(connection1))
            {
                conn.Open();

                foreach (var book in selectedBooks)
                {
                    string query = "INSERT INTO saver1 (iduser, bookname, author, price) VALUES (@user, @name, @auth, @price)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", id);
                        cmd.Parameters.AddWithValue("@name", book.Name);
                        cmd.Parameters.AddWithValue("@auth", book.author);
                        cmd.Parameters.AddWithValue("@price", book.price);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}
