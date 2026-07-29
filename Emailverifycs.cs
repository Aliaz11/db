using System.Security.Cryptography;
using System.Text;
using db.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace db
{
    internal class Emailverifycs : IEmailveri
    {
        /// <summary>How long a freshly sent verification code stays usable.</summary>
        private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);

        private string? _code;
        private DateTimeOffset _codeExpiresAtUtc;

        /// <summary>
        /// Generates a fresh code and mails it. Kept for the existing <c>Form8</c> call site;
        /// use <see cref="SendCode"/> when you need to know whether the mail actually went out.
        /// </summary>
        public void EmailSender(string UserEntry)
        {
            SendCode(UserEntry);
        }

        /// <summary>
        /// Generates a new 6-digit code and sends it to <paramref name="emailAddress"/>.
        /// Returns true only when the message was handed to the SMTP server.
        /// Any previous code is replaced, so a resend always invalidates the old one.
        /// </summary>
        public bool SendCode(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                MessageBox.Show("Please enter an email address first.", "Email verification",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!AppSettings.IsSmtpConfigured)
            {
                MessageBox.Show(
                    "Email sending is not configured, so no verification code was sent." +
                    Environment.NewLine + Environment.NewLine +
                    "Set these environment variables and restart the application:" +
                    Environment.NewLine +
                    "    SMTP_HOST      - SMTP server, e.g. smtp.gmail.com" + Environment.NewLine +
                    "    SMTP_PORT      - SMTP port, e.g. 587" + Environment.NewLine +
                    "    SMTP_USERNAME  - the mailbox used to authenticate" + Environment.NewLine +
                    "    SMTP_PASSWORD  - an app password, never the normal account password" + Environment.NewLine +
                    "    SMTP_FROM      - optional sender address (defaults to SMTP_USERNAME)",
                    "Email not configured", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Invalidate whatever was outstanding before we try to send a replacement.
            _code = null;

            string code = GenerateCode();

            try
            {
                string fromAddress = string.IsNullOrWhiteSpace(AppSettings.SmtpFrom)
                    ? AppSettings.SmtpUserName
                    : AppSettings.SmtpFrom;

                using var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(fromAddress));
                message.To.Add(MailboxAddress.Parse(emailAddress.Trim()));
                message.Subject = "Email Verification";
                message.Body = new TextPart("plain")
                {
                    Text = "Your verification code is: " + code + Environment.NewLine +
                           "It expires in " + (int)CodeLifetime.TotalMinutes + " minutes."
                };

                using var client = new SmtpClient();
                client.Connect(
                    AppSettings.SmtpHost,
                    AppSettings.SmtpPort,
                    AppSettings.SmtpUseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
                client.Authenticate(AppSettings.SmtpUserName, AppSettings.SmtpPassword);
                client.Send(message);
                client.Disconnect(true);
            }
            catch (ParseException)
            {
                MessageBox.Show("That does not look like a valid email address.", "Email verification",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (AuthenticationException)
            {
                MessageBox.Show(
                    "The SMTP server rejected the configured credentials. Check SMTP_USERNAME and SMTP_PASSWORD.",
                    "Email verification failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("The verification email could not be sent: " + ex.Message,
                    "Email verification failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _code = code;
            _codeExpiresAtUtc = DateTimeOffset.UtcNow.Add(CodeLifetime);
            return true;
        }

        /// <summary>
        /// Fixed-time comparison of <paramref name="code"/> against the outstanding code.
        /// Empty input, an expired code and an already-used code all return false.
        /// A successful verification consumes the code.
        /// </summary>
        public bool TryVerify(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            string? expected = _code;
            if (string.IsNullOrEmpty(expected))
                return false;

            if (DateTimeOffset.UtcNow > _codeExpiresAtUtc)
            {
                _code = null;
                return false;
            }

            bool matches = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(code.Trim()),
                Encoding.UTF8.GetBytes(expected));

            if (matches)
                _code = null;

            return matches;
        }

        public void adapt(TextBox textbox1, Form form, Form this_form)
        {
            if (TryVerify(textbox1.Text))
            {
                MessageBox.Show("The code was correct.", "Email verification",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Read the geometry before closing, otherwise this_form is already disposed.
                form.Location = this_form.Location;
                form.Size = this_form.Size;
                form.StartPosition = FormStartPosition.Manual;

                this_form.Close();
                form.Show();
            }
            else
            {
                MessageBox.Show("The code is wrong, already used, or has expired. Request a new one.",
                    "Email verification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>Cryptographically random, fixed-width 6-digit code (000000-999999).</summary>
        private static string GenerateCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000)
                .ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
