namespace db
{
    public interface IEmailveri
    {
        /// <summary>Generates and sends a fresh verification code, ignoring the outcome.</summary>
        void EmailSender(string userEntry);

        /// <summary>Generates and sends a fresh verification code. True when the mail was sent.</summary>
        bool SendCode(string emailAddress);

        /// <summary>True when the supplied code matches the outstanding, unexpired code.</summary>
        bool TryVerify(string code);

        void adapt(TextBox textbox1, Form nextForm, Form currentForm);
    }
}
