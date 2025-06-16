using System.Net;
using System.Net.Mail;

namespace db
{
    internal class Emailverifycs : IEmailveri
    {
        int rand1;
        string connection1 = Locator.GetConnectionString();
        Random random = new Random();
        public Emailverifycs()
        {
            rand1 = random.Next(1000000, 99999999);

        }
        public void EmailSender(string UserEntry)
        {

            try
            {


                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");


                mail.From = new MailAddress("alitry679@gmail.com");


                mail.To.Add(UserEntry);

                mail.Subject = "Email Verification";
                mail.Body = "Your verification code is: " + rand1;


                smtpClient.Port = 587;
                smtpClient.UseDefaultCredentials = false;


                smtpClient.Credentials = new NetworkCredential("alitry679@gmail.com", "xoki izwv ddpp lyku");
                smtpClient.EnableSsl = true;


                smtpClient.Send(mail);

                Console.WriteLine("Verification email sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending verification email: " + ex.Message);
            }


        }
        public void adapt(TextBox textbox1, Form form, Form this_form)
        {
            if (textbox1.Text == rand1.ToString())
            {
                MessageBox.Show("it was correct");
                this_form.Close();


                form.Location = this_form.Location;
                form.Size = this_form.Size;
                form.StartPosition = FormStartPosition.Manual;
                form.Show();

                form.Show();


            }
            else
            {
                MessageBox.Show("it was wrong");

            }
        }




        public void EmailSender()
        {
            throw new NotImplementedException();
        }
    }
}

