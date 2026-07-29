using System.Data;
using Microsoft.Data.SqlClient;
using db.Configuration;
using db.Security;

namespace db
{
    public class DataBaseCrud : IDataBaseCrud
    {
        /// <summary>Placeholder shown instead of the stored password hash in the user list view.</summary>
        public const string MaskedPassword = "********";

        private readonly string connection;

        public DataBaseCrud() : this(AppSettings.ConnectionString)
        {
        }

        public DataBaseCrud(string conection_string)
        {
            connection = string.IsNullOrWhiteSpace(conection_string)
                ? AppSettings.ConnectionString
                : conection_string;
        }

        public byte[] getphoto(PictureBox picturebox)
        {
            if (picturebox.Image == null)
            {
                return Array.Empty<byte>();
            }

            using (MemoryStream stream = new MemoryStream())
            {
                picturebox.Image.Save(stream, picturebox.Image.RawFormat);
                return stream.ToArray();
            }
        }

        public void update(DataGridView dataGridView1, int index, TextBox textBox1, TextBox textBox2, TextBox textBox3, NumericUpDown numeric, DateTimePicker datatime)
        {
            if (index < 0 || index >= dataGridView1.Rows.Count)
            {
                MessageBox.Show("Please select a data row first.");
                return;
            }

            DataGridViewRow row = dataGridView1.Rows[index];

            textBox1.Text = row.Cells["name"].Value?.ToString() ?? "";
            textBox2.Text = row.Cells["author"].Value?.ToString() ?? "";
            textBox3.Text = row.Cells["price"].Value?.ToString() ?? "";

            if (row.Cells["quantity"].Value != DBNull.Value
                && decimal.TryParse(row.Cells["quantity"].Value?.ToString(), out var qty))
            {
                numeric.Value = qty;
            }
            else
            {
                numeric.Value = 0;
            }

            if (row.Cells["Date"].Value != DBNull.Value
                && DateTime.TryParse(row.Cells["Date"].Value?.ToString(), out var dt))
            {
                datatime.Value = dt;
            }
        }

        public void updateBase(DataGridView dataGridView1, int index, TextBox textBox1, TextBox textBox2, TextBox textBox3, NumericUpDown numeric, DateTimePicker datatime)
        {
            if (index < 0 || index >= dataGridView1.Rows.Count)
            {
                MessageBox.Show("Please select a data row first.");
                return;
            }

            if (!TryGetRowId(dataGridView1.Rows[index], out int id))
            {
                return;
            }

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                const string query = "UPDATE Books SET name = @name, author = @author, price = @price, quantity = @quantity, Date = @Date WHERE ID = @id";

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@name", textBox1.Text);
                    command.Parameters.AddWithValue("@author", textBox2.Text);
                    command.Parameters.AddWithValue("@price", textBox3.Text);
                    command.Parameters.AddWithValue("@quantity", numeric.Value);
                    command.Parameters.AddWithValue("@Date", datatime.Value);
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
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

                if (!TryGetRowId(dataGridView1.Rows[index], out int id))
                {
                    return;
                }

                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();

                    const string query = "DELETE FROM Books WHERE Id = @id";

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            if (index >= 0 && index < dataGridView1.Rows.Count)
                            {
                                dataGridView1.Rows.RemoveAt(index);
                                MessageBox.Show("Book deleted successfully from database and table.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public void selector(DataGridView dataGridView1, Form form)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                string query = "SELECT * FROM Books";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnection))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                    dataGridView1.AllowUserToAddRows = false;
                    if (dataGridView1.Columns.Contains("image"))
                    {
                        DataGridViewImageColumn imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["image"]!;
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Stretch;
                    }
                    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                    chk.HeaderText = "Select";
                    chk.Name = "chk";
                    if (form is Form5)
                    {
                        dataGridView1.Columns.Insert(0, chk);
                    }
                }
            }
        }

        /// <summary>
        /// Fills the user list view. Column order is Id, firstname, lastname, phonenumber, birthdate,
        /// email, gender, password, username - but the password column always shows
        /// <see cref="MaskedPassword"/>, never the stored hash.
        /// </summary>
        public void selector(ListView listView1)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();

                    string query = "SELECT * FROM Stu1";

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        listView1.Items.Clear();

                        while (reader.Read())
                        {
                            ListViewItem item = new ListViewItem(reader["Id"].ToString() ?? "");

                            item.SubItems.Add(reader["firstname"].ToString() ?? "");
                            item.SubItems.Add(reader["lastname"].ToString() ?? "");
                            item.SubItems.Add(reader["phonenumber"].ToString() ?? "");
                            item.SubItems.Add(reader["birthdate"].ToString() ?? "");
                            item.SubItems.Add(reader["email"].ToString() ?? "");
                            item.SubItems.Add(reader["gender"].ToString() ?? "");
                            item.SubItems.Add(MaskedPassword);
                            item.SubItems.Add(reader["username"].ToString() ?? "");

                            listView1.Items.Add(item);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                // Never surface ex.ToString(): the stack trace carries the connection string.
                MessageBox.Show("Could not load the user list from the database. Please try again.",
                    "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Updates the selected users. When <paramref name="password"/> is null or whitespace
        /// (or still the masked placeholder) the Password column is left untouched; otherwise the
        /// new password is stored hashed.
        /// </summary>
        public void update(ListView listView1, string firstname, string lastname, string phonenumber1, string gender, string Birthdate, string password, string email, string username)
        {
            bool changePassword = !string.IsNullOrWhiteSpace(password)
                && !string.Equals(password, MaskedPassword, StringComparison.Ordinal);

            string setClause = "firstname = @firstname, lastname = @lastname, phonenumber = @phonenumber, gender = @gender, Birthdate = @Birthdate, username = @username, Email = @email";
            if (changePassword)
            {
                setClause += ", Password = @password";
            }

            string query = "UPDATE Stu1 SET " + setClause + " WHERE ID = @id";

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                bool updated = false;

                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    if (!int.TryParse(item.SubItems[0].Text, out int id))
                    {
                        continue;
                    }

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@firstname", firstname);
                        command.Parameters.AddWithValue("@gender", gender);
                        command.Parameters.AddWithValue("@lastname", lastname);
                        command.Parameters.AddWithValue("@phonenumber", phonenumber1);
                        command.Parameters.AddWithValue("@Birthdate", Birthdate);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        if (changePassword)
                        {
                            command.Parameters.AddWithValue("@password", PasswordHasher.Hash(password));
                        }

                        if (command.ExecuteNonQuery() > 0)
                        {
                            updated = true;
                        }
                    }
                }

                if (updated)
                {
                    selector(listView1);
                }
            }
        }

        public void delete(ListView listView1)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                const string query = "DELETE FROM Stu1 WHERE Id = @id";

                foreach (ListViewItem item in listView1.SelectedItems.Cast<ListViewItem>().ToList())
                {
                    if (!int.TryParse(item.SubItems[0].Text, out int id))
                    {
                        continue;
                    }

                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            listView1.Items.Remove(item);
                        }
                    }
                }
            }
        }

        /// <summary>Inserts a user. The password is always stored hashed, never in plaintext.</summary>
        public void insert(
              string firstname,
              string lastname,
              string phonenumber,
              string birthdate,
              string email,
              string gender,
              string password,
              string username,
            byte[] photoer)
        {
            const string query = "INSERT INTO Stu1(firstname,lastname,phonenumber,Birthdate,Email,Gender,Password,username,image)VALUES(@firstname,@lastname,@phonenumber,@birthdate,@email,@gender,@password,@username,@image)";

            using (SqlConnection sqlconnection2 = new SqlConnection(connection))
            {
                sqlconnection2.Open();

                using (SqlCommand command2 = new SqlCommand(query, sqlconnection2))
                {
                    command2.Parameters.AddWithValue("@firstname", firstname);
                    command2.Parameters.AddWithValue("@lastname", lastname);
                    command2.Parameters.AddWithValue("@phonenumber", phonenumber);
                    command2.Parameters.AddWithValue("@birthdate", birthdate);
                    command2.Parameters.AddWithValue("@email", email);
                    command2.Parameters.AddWithValue("@gender", gender);
                    command2.Parameters.AddWithValue("@password", PasswordHasher.Hash(password));
                    command2.Parameters.AddWithValue("@username", username);
                    command2.Parameters.AddWithValue("@image", (object?)photoer ?? DBNull.Value);
                    command2.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Reads the primary key from the first cell of a grid row, reporting a clear message when it is missing.</summary>
        private static bool TryGetRowId(DataGridViewRow row, out int id)
        {
            id = 0;

            string? raw = row.Cells[0].Value?.ToString();
            if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out id))
            {
                MessageBox.Show("The selected row has no valid ID.", "Invalid selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}
