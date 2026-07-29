namespace db
{
    internal interface IPasswordchange
    {
        /// <summary>Returns true when the password was validated and stored.</summary>
        bool updator(TextBox newPassword, TextBox confirmPassword);
    }
}
