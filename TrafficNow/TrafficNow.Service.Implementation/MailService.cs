using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class MailService : IMailService
    {

        public bool SendMail(string fromAddress, string toAddress, string subject, string body)
        {
            

            // Supply your SMTP credentials below. Note that your SMTP credentials are different from your AWS credentials.
            const String SMTP_USERNAME = "digbuzzi";  // Replace with your SMTP username.
            const String SMTP_PASSWORD = "CQ8G8f62JBH42gmRdx6nsw";  // Replace with your SMTP password.

            // Amazon SES SMTP host name. This example uses the US West (Oregon) region.
            const String HOST = "smtp.mandrillapp.com";

            // Port we will connect to on the Amazon SES SMTP endpoint. We are choosing port 587 because we will use
            // STARTTLS to encrypt the connection.
            const int PORT = 587;

            // Create an SMTP client with the specified host name and port.
            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromAddress);
                message.To.Add(toAddress);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                // Create a network credential with your SMTP user name and password.
                client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                client.EnableSsl = true;
                

                // Send the email. 
                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                   
                }
            }
            return false;
        }
    }
    
}
