using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Data.SqlClient;
using WinFormsApp3;

namespace db
{

    public class ValidationError
    {

        public string FieldName { get; }
        public string Message { get; }
        public Point Position { get; }

        public ValidationError(string fieldName, string message, Point position)
        {
            FieldName = fieldName;
            Message = message;
            Position = position;
        }
    }


    public class LabelValidator
    {

        public string connection = Locator.GetConnectionString();
        private  RequiredValidDel _required = CommonFieldValidatorFunctions.RequiredFieldValidDel;
        private  StringLengthValidDel _length = CommonFieldValidatorFunctions.StringLengthFieldValidDel;
        private  PatternMatchValidDel _pattern = CommonFieldValidatorFunctions.PatternMatchValidDel;
        private  CompareFieldsValidDel _compare = CommonFieldValidatorFunctions.FieldsCompareValidDel;

        /// <summary>
        /// Checks whether the username or the email is already taken and clears the offending textbox.
        /// Blank values are never treated as a collision, so several users may have no email at all.
        /// </summary>
        public void selectoring(string email, string username, TextBox text_email, TextBox text_user)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();

                    if (!string.IsNullOrWhiteSpace(username) &&
                        Exists(sqlConnection, "SELECT COUNT(1) FROM Stu1 WHERE username = @value", username))
                    {
                        MessageBox.Show("the user name exists");
                        if (text_user != null)
                            text_user.Text = "";
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(email) &&
                        Exists(sqlConnection, "SELECT COUNT(1) FROM Stu1 WHERE Email = @value", email))
                    {
                        MessageBox.Show("the email already exists");
                        if (text_email != null)
                            text_email.Text = "";
                    }
                }
            }
            catch (SqlException)
            {
                // Never surface ex.ToString(): the stack trace carries the connection string.
                MessageBox.Show("Could not check the username and email against the database. Please try again.",
                    "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Could not open a connection to the database. Please try again.",
                    "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Security", "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Both call sites pass a compile-time string literal; the only user-supplied " +
                            "value is bound through the @value SqlParameter below. The analyser cannot " +
                            "follow the literal across the method parameter.")]
        private static bool Exists(SqlConnection sqlConnection, string query, string value)
        {
            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                command.Parameters.Add("@value", System.Data.SqlDbType.NVarChar, 4000).Value = value;
                object? scalar = command.ExecuteScalar();
                return scalar != null && scalar != DBNull.Value && Convert.ToInt32(scalar) > 0;
            }
        }


        public IEnumerable<ValidationError> Validate(IUser user)
        {
            if (!_required(user.FirstName))
                yield return new ValidationError("label_firstname", "First name cannot be empty", new Point(300, 80));
            else if (!_length(user.FirstName, 2, 11))
                yield return new ValidationError("label_firstname", "First name must be 2–11 letters", new Point(300, 80));

            if (!_required(user.LastName))
                yield return new ValidationError("label_lastname", "Last name cannot be empty", new Point(300, 150));
            else if (!_length(user.LastName, 2, 11))
                yield return new ValidationError("label_lastname", "Last name must be 2 to 11 letters", new Point(300, 150));

            if (string.IsNullOrEmpty(user.Email) && string.IsNullOrEmpty(user.PhoneNumber))
                yield return new ValidationError("label_phone", "You must provide either a phone or an email", new Point(300, 220));
            else if (!string.IsNullOrEmpty(user.PhoneNumber) && !_length(user.PhoneNumber, 10, 11))
                yield return new ValidationError("label_phone", "Phone number must be 10–11 digits", new Point(300, 220));

            if (string.IsNullOrEmpty(user.Gender))
                yield return new ValidationError("label_gender", "Gender must be chosen", new Point(300, 300));

            // All three password branches use the same field name so that the previously shown
            // label is actually removed on the next validation pass.
            if (!_required(user.Password))
                yield return new ValidationError("label_password", "Password must be entered", new Point(300, 400));
            else if (!_pattern(user.Password, Regex.Strong_Password_RegEx_Pattern))
                yield return new ValidationError("label_password", "Password must be 8–64 characters with upper, lower, digit and special char, and no spaces", new Point(300, 400));
            else if (!_compare(user.Password, user.PasswordR))
                yield return new ValidationError("label_password", "Passwords don’t match", new Point(300, 400));
        }
        public static void RemoveValidationLabel(string name,Form form)
        {
            var matches = form.Controls.Find(name, true);
            foreach (var c in matches)
                form.Controls.Remove(c);
        }
        public static void ShowValidationLabel(string name, string message, Point pos,Form form)
        {

            var old = form.Controls.Find(name, true);
            foreach (var c in old)
            {

                form.Controls.Remove(c);
            }


            var lbl = new Label
            {
                Name = name,
                Text = message,
                BackColor = Color.Red,
                Location = pos,
                AutoSize = true
            };
            form.Controls.Add(lbl);
        }
    }
}
