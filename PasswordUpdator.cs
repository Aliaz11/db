using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WinFormsApp3;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace db
{
    public class PasswordUpdator: IPasswordchange
    {
        string connection=Locator.GetConnectionString();
        string email;
        public PasswordUpdator(string email,Form form)
        {
            this.email = email;

        }
        public void updator(TextBox textBox1,TextBox textBox2)
        {
            if ((!CommonFieldValidatorFunctions.FieldPatternValid(textBox1.Text, Regex.Strong_Password_RegEx_Pattern)))
            {
                MessageBox.Show("the password isnt strong enough");
                return;

            }
            else if (!CommonFieldValidatorFunctions.PatternMatchValidDel(textBox1.Text, textBox2.Text))

            {
                MessageBox.Show("password dont match");

                return;
            }
            else
            {
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();




                    string query = $"UPDATE Stu1 SET password=@password WHERE Email = '{email}'";

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {



                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@password", textBox2.Text);
                        command.ExecuteNonQuery();





                    }
                    sqlConnection.Close();
                    Form4 form = new Form4();
                    form.Close();
                    form.Show();
                }
            }

        }
    }
}
