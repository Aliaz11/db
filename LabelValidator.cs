using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Asn1.Crmf;
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
        public void selectoring(string email,string username,TextBox text_email,TextBox text_user)
        {
            try
            {

                SqlConnection sqlConnection = new SqlConnection(connection);
                sqlConnection.Open();
                string query = "SELECT username,email FROM Stu1";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                     string user1 =reader["username"].ToString();
                    string email1 = reader["email"].ToString();
                    if (user1==username)
                    {
                        MessageBox.Show("the user name exists");
                        text_user.Text = "";
                        return;
                    }
                    else if (email1==email)
                    {
                        if (string.IsNullOrEmpty(email1))
                        {
                            continue;
                        }
                        MessageBox.Show("the email already exists");
                        text_email.Text = "";
                        return;
                    }


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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

            if (!_required(user.Password))
                yield return new ValidationError("label_password", "Password must be entered", new Point(300, 400));
            else if (!_pattern(user.Password, Regex.Strong_Password_RegEx_Pattern))
                yield return new ValidationError("label_password1", "Password must have uppercase, lowercase and special char", new Point(300, 400));
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
