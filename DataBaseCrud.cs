using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;





namespace db
{
    public class DataBaseCrud
    {
        string connection = Locator.GetConnectionString();
        public byte[] getphoto(PictureBox picturebox)
        {
            MemoryStream stream = new MemoryStream();
            picturebox.Image.Save(stream, picturebox.Image.RawFormat);
            return stream.GetBuffer();
        }
        string conection_string;
        
        public DataBaseCrud(string conection_string) {

            this.conection_string = conection_string;
        }

        public void delete(DataGridView dataGridView1, int index,TextBox textBox1,TextBox textBox2,TextBox textBox3)   
        {
            try
            {
                DataGridViewRow row = dataGridView1.Rows[index];
                string id = row.Cells[1].Value.ToString();


                textBox1.Text = row.Cells[1].Value.ToString();
                textBox2.Text = row.Cells[2].Value.ToString();
                textBox3.Text = row.Cells[3].Value.ToString();
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();


                    string query = $"DELETE FROM Books WHERE Id = {id}";

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {



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
        
        public void selector(DataGridView dataGridView1,Form form)
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
                    if (dataGridView1.Columns.Contains("image"))
                    {
                        DataGridViewImageColumn imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["image"];
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
        public void selector(ListView listView1)
        {
            try
            {
                string connection_string = connection;
                SqlConnection sqlConnection = new SqlConnection(connection);
                sqlConnection.Open();
                string query = "SELECT * FROM Stu1";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();
                listView1.Items.Clear();

                while (reader.Read())
                {

                    ListViewItem item = new ListViewItem(reader["Id"].ToString());

                    item.SubItems.Add(reader["firstname"].ToString());

                    item.SubItems.Add(reader["lastname"].ToString());
                    item.SubItems.Add(reader["phonenumber"].ToString());
                    item.SubItems.Add(reader["birthdate"].ToString());
                    item.SubItems.Add(reader["email"].ToString());
                    item.SubItems.Add(reader["gender"].ToString());
                    item.SubItems.Add(reader["password"].ToString());
                    item.SubItems.Add(reader["username"].ToString());





                    listView1.Items.Add(item);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            
        }
        public void update(ListView listView1, string firstname, string lastname, string phonenumber1, string gender, string Birthdate, string password)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();   
         
         
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                    string id = item.SubItems[0].Text;

                    string query = $"UPDATE Stu1 SET firstname = @firstname, lastname = @lastname, phonenumber = @phonenumber,gender=@gender,Birthdate=@Birthdate,password=@password WHERE ID = '{id}'";

                        using (SqlCommand command = new SqlCommand(query, sqlConnection))
                        {


                            command.Parameters.AddWithValue("@firstname", firstname);
                            command.Parameters.AddWithValue("@gender", gender);
                            command.Parameters.AddWithValue("@lastname", lastname);
                            command.Parameters.AddWithValue("@phonenumber", phonenumber1);
                            command.Parameters.AddWithValue("@Birthdate", Birthdate);
                            command.Parameters.AddWithValue("@password", password);


                            if (command.ExecuteNonQuery() > 0)
                            {
                                selector(listView1);
                            }
                        }
                    }
                    sqlConnection.Close();

                }
            }
        
        public void delete(ListView listView1)
        {
            string connection_string = connection;
            SqlConnection sqlConnection = new SqlConnection(connection);
            sqlConnection.Open();

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                string id= item.SubItems[0].Text;
       



                string query = $"DELETE FROM Stu1 WHERE Id = {id}";
                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                int rows = cmd.ExecuteNonQuery();
              



                if (rows > 0)
                {
                    listView1.Items.Remove(item);
                }

            }

            sqlConnection.Close();
        }

        
        public void insert(
              string firstname,
              string lastname,
              string phonenumber,
              string birthdate,
              string email,
              string gender,
              string password,
              string username,
             Func<byte[]> getphoto)
        {
            string query = $"INSERT INTO Stu1(firstname,lastname,phonenumber,Birthdate,Email,Gender,Password,username,image)VALUES(@firstname,@lastname,@phonenumber,@birthdate,@email,@gender,@password,@username,@image)";

            using (SqlConnection sqlconnection2 = new SqlConnection(connection))
            {
                sqlconnection2.Open();

                SqlCommand command2 = new SqlCommand(query, sqlconnection2);
                command2.Parameters.AddWithValue("@firstname", firstname);
                command2.Parameters.AddWithValue("@lastname", lastname);
                command2.Parameters.AddWithValue("@phonenumber", phonenumber);
                command2.Parameters.AddWithValue("@birthdate", birthdate);
                command2.Parameters.AddWithValue("@email", email);
                command2.Parameters.AddWithValue("@gender", gender);
                command2.Parameters.AddWithValue("@password", password);
                command2.Parameters.AddWithValue("@username", username);
                command2.Parameters.AddWithValue("@image", getphoto());
                command2.ExecuteNonQuery();
                sqlconnection2.Close();
             

            }
        }

    }
}
