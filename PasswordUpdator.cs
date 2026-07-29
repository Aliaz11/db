using System.Data;
using Microsoft.Data.SqlClient;
using WinFormsApp3;
using db.Configuration;
using db.Security;

namespace db
{
    public class PasswordUpdator : IPasswordchange
    {
        private readonly string connection = AppSettings.ConnectionString;
        private readonly string email;

        public PasswordUpdator(string email, Form form)
        {
            this.email = email;
        }

        /// <summary>
        /// Validates and stores a new (hashed) password for the account this instance was created for.
        /// Returns true only when a row was actually updated; navigation is left to the caller.
        /// </summary>
        public bool updator(TextBox newPassword, TextBox confirmPassword)
        {
            if (!CommonFieldValidatorFunctions.FieldPatternValid(newPassword.Text, Regex.Strong_Password_RegEx_Pattern))
            {
                MessageBox.Show("the password isnt strong enough");
                return false;
            }

            if (!CommonFieldValidatorFunctions.FieldsCompareValidDel(newPassword.Text, confirmPassword.Text))
            {
                MessageBox.Show("password dont match");
                return false;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("No account is associated with this request.");
                return false;
            }

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                const string query = "UPDATE Stu1 SET Password = @password WHERE Email = @email";

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@password", PasswordHasher.Hash(newPassword.Text));
                    command.Parameters.Add("@email", SqlDbType.NVarChar, 200).Value = email;

                    if (command.ExecuteNonQuery() <= 0)
                    {
                        MessageBox.Show("No account was found for this email address.");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
