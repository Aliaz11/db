using System.Data;
using Microsoft.Data.SqlClient;
using db.Configuration;

namespace db
{
    public class DbCrudBook : IDbCrudBook
    {
        private readonly string connection1;
        private readonly string id;

        /// <summary>
        /// Creates the basket repository for one user.
        /// </summary>
        /// <param name="id">The <c>Stu1.Id</c> of the signed-in user - stored in <c>saver1.iduser</c>. This is NOT a connection string.</param>
        public DbCrudBook(string id) : this(id, AppSettings.ConnectionString)
        {
        }

        /// <summary>
        /// Creates the basket repository for one user against an explicit database.
        /// </summary>
        /// <param name="id">The <c>Stu1.Id</c> of the signed-in user - stored in <c>saver1.iduser</c>.</param>
        /// <param name="connectionString">The database connection string.</param>
        public DbCrudBook(string id, string connectionString)
        {
            this.id = id ?? "";
            connection1 = string.IsNullOrWhiteSpace(connectionString)
                ? AppSettings.ConnectionString
                : connectionString;
        }

        public void insert(Book book)
        {
            const string query = "INSERT INTO Books(name,author,price,image,quantity,Date)VALUES(@name,@author,@price,@image,@quantity,@Date)";

            using (SqlConnection sqlconnection2 = new SqlConnection(connection1))
            {
                sqlconnection2.Open();

                using (SqlCommand command2 = new SqlCommand(query, sqlconnection2))
                {
                    command2.Parameters.AddWithValue("@name", book.Name);
                    command2.Parameters.AddWithValue("@author", book.author);
                    command2.Parameters.AddWithValue("@price", book.price);
                    command2.Parameters.AddWithValue("@image", (object?)book.image ?? DBNull.Value);
                    command2.Parameters.AddWithValue("@quantity", book.quantity);
                    command2.Parameters.AddWithValue("@Date", book.Date);
                    command2.ExecuteNonQuery();
                }
            }
        }

        public void inserter(DataGridView dataGridView1)
        {
            List<Book> booker = new List<Book>();
            decimal total = 0m;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool IsChecked = Convert.ToBoolean(row.Cells["chk"].Value);
                if (!IsChecked)
                {
                    continue;
                }

                Book book2 = new Book();
                book2.Name = row.Cells["name"].Value?.ToString() ?? "";
                book2.author = row.Cells["author"].Value?.ToString() ?? "";
                book2.price = row.Cells["price"].Value?.ToString() ?? "";
                book2.image = row.Cells["image"].Value as byte[] ?? Array.Empty<byte>();

                if (decimal.TryParse(book2.price, out decimal price))
                {
                    total += price;
                }

                booker.Add(book2);
            }

            if (booker.Count == 0)
            {
                MessageBox.Show("No books selected.");
                return;
            }

            MessageBox.Show("the price to pay: " + total);

            using (SqlConnection conn = new SqlConnection(connection1))
            {
                conn.Open();

                foreach (var book in booker)
                {
                    const string checkQuery = "SELECT COUNT(*) FROM saver1 WHERE iduser = @user AND bookname = @name AND author = @auth AND price = @price";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@user", id);
                        checkCmd.Parameters.AddWithValue("@name", book.Name);
                        checkCmd.Parameters.AddWithValue("@auth", book.author);
                        checkCmd.Parameters.AddWithValue("@price", book.price);

                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            continue;
                        }
                    }

                    const string query = "INSERT INTO saver1 (iduser, bookname, author, price,image) VALUES (@user, @name, @auth, @price,@image)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", id);
                        cmd.Parameters.AddWithValue("@name", book.Name);
                        cmd.Parameters.AddWithValue("@auth", book.author);
                        cmd.Parameters.AddWithValue("@price", book.price);
                        cmd.Parameters.AddWithValue("@image", (object?)book.image ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void delete(DataGridView dataGridView1, int index)
        {
            try
            {
                if (index < 0 || index >= dataGridView1.Rows.Count)
                {
                    MessageBox.Show("Please select a data row first.");
                    return;
                }

                DataGridViewRow row = dataGridView1.Rows[index];
                string? raw = row.Cells[0].Value?.ToString();

                if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out int rowId))
                {
                    MessageBox.Show("The selected row has no valid ID.", "Invalid selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection sqlConnection = new SqlConnection(connection1))
                {
                    sqlConnection.Open();

                    const string query1 = "DELETE FROM saver1 WHERE Id = @id";

                    using (SqlCommand command = new SqlCommand(query1, sqlConnection))
                    {
                        command.Parameters.Add("@id", SqlDbType.Int).Value = rowId;

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            if (index >= 0 && index < dataGridView1.Rows.Count)
                            {
                                dataGridView1.Rows.RemoveAt(index);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No book found with the selected ID or deletion failed in the database.", "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error during deletion: {ex.Message}\nError Code: {ex.Number}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "General Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
