using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailKit;

using MimeKit;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace db
{
    public partial class Form8 : Form
    {
        int random;
        public Form8()
        {
            InitializeComponent();
        }
        Random rand = new Random();
        private void button1_Click(object sender, EventArgs e)
        {
             random = rand.Next(100000, 999000);
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");


                mail.From = new MailAddress("alitry679@gmail.com");


                mail.To.Add(textBox1.Text);

                mail.Subject = "Email Verification";
                mail.Body = "Your verification code is: " + random;


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


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.ToString() == random.ToString())
            {
                MessageBox.Show("right");

            }
            else
            {
                MessageBox.Show("the password was wrong");
            }
        }
    }
}
