using Microsoft.Data.SqlClient;

namespace db
{
    public class DbCrudBook:IDbCrudBook
    {
        int price1 = 0;
        string connection1 = Locator.GetConnectionString();
        string id = "";
        public DbCrudBook(string id)
        {
            this.id = id;

        }
        public void insert(Book book)
        {
            string query = $"INSERT INTO Books(name,author,price,image,quantity,Date)VALUES(@name,@author,@price,@image,@quantity,@Date)";
            using (SqlConnection sqlconnection2 = new SqlConnection(connection1))
            {
                sqlconnection2.Open();

                SqlCommand command2 = new SqlCommand(query, sqlconnection2);
                command2.Parameters.AddWithValue("@name", book.Name);
                command2.Parameters.AddWithValue("@author", book.author);
                command2.Parameters.AddWithValue("@price", book.price);
                command2.Parameters.AddWithValue("@image", book.image);
                command2.Parameters.AddWithValue("@quantity", book.quantity);
                command2.Parameters.AddWithValue("@Date", book.Date);


                command2.ExecuteNonQuery();
                sqlconnection2.Close();


            }
        }
        public void inserter(DataGridView dataGridView1)
        {
            List<Book>booker = new List<Book>();
            Book book2 = new Book();


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {

      


                bool IsChecked = Convert.ToBoolean(row.Cells["chk"].Value);
                if (IsChecked)
                {


                    book2.Name = row.Cells["name"].Value?.ToString() ?? "";
                    book2.author = row.Cells["author"].Value?.ToString() ?? "";
                    book2.price = row.Cells["price"].Value?.ToString() ?? "";
                    book2.image = (byte[])row.Cells["image"].Value;
                    price1 += Convert.ToInt32(book2.price);
                    booker.Add(book2);


                }
            }
            MessageBox.Show("the price to pay: " + price1);

            if (booker.Count == 0)
            {
                MessageBox.Show("No books selected.");
                return;
            }



            using (SqlConnection conn = new SqlConnection(connection1))
            {
                conn.Open();
                


                foreach (var book in booker)
                {
                    string checkQuery = "SELECT COUNT(*) FROM saver1 WHERE iduser = @user AND bookname = @name AND author = @auth AND price = @price";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@user", id);
                        checkCmd.Parameters.AddWithValue("@name", book.Name);
                        checkCmd.Parameters.AddWithValue("@auth", book.author);
                        checkCmd.Parameters.AddWithValue("@price", book.price);

                        int exists = (int)checkCmd.ExecuteScalar();
                        if (exists > 0)
                        {
                            continue;
                        }
                        string query = "INSERT INTO saver1 (iduser, bookname, author, price,image) VALUES (@user, @name, @auth, @price,@image)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@user", id);
                            cmd.Parameters.AddWithValue("@name", book.Name);
                            cmd.Parameters.AddWithValue("@auth", book.author);
                            cmd.Parameters.AddWithValue("@price", book.price);
                            cmd.Parameters.AddWithValue("@image", book.image);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

        }
        public void delete(DataGridView dataGridView1, int index)
        {
            try
            {
                DataGridViewRow row = dataGridView1.Rows[index];
                string id = row.Cells[0].Value?.ToString() ?? "";

                using (SqlConnection sqlConnection = new SqlConnection(connection1))
                {
                    sqlConnection.Open();


                    string query1 = $"DELETE FROM saver1 WHERE Id = {id}";

                    using (SqlCommand command = new SqlCommand(query1, sqlConnection))
                    {



                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {

                            if (index >= 0 && index < dataGridView1.Rows.Count)
                            {
                                dataGridView1.Rows.RemoveAt(index);
                                MessageBox.Show("deleted");






                            }
                        }
                        else
                        {
                            MessageBox.Show("No book found with the selected ID or deletion failed in the database.", "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }



                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}