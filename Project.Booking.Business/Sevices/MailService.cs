using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Business.Sevices
{
    public class MailService
    {
        public void SendMail(Email email)
        {
            try
            {
                if (email.To.Count > 0)
                {
                    //email.Body = System.Net.WebUtility.HtmlDecode("&#35;");

                    var msg = new MailMessage(
                          email.from,
                          string.Join(",", email.To.ToArray()),                          
                          email.Subject,
                         email.Body
                          );                    

                    msg.IsBodyHtml = true;

                    if (email.Bcc.Count > 0)
                    {
                        msg.Bcc.Add(string.Join(",", email.Bcc.ToArray()));
                    }

                    if (email.CC.Count > 0)
                    {
                        msg.CC.Add(string.Join(",", email.CC.ToArray()));
                    }

                    var client = new SmtpClient(email.host, email.port)
                    {
                        Credentials = new NetworkCredential(email.username, email.password),
                        EnableSsl = true
                    };

                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //client.UseDefaultCredentials = false;
                    //client.Timeout = 120000;

                    //Add this line to bypass the certificate validation
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                            System.Security.Cryptography.X509Certificates.X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };
                    client.Send(msg);
                }
            }
            catch (Exception ex)
            {
               
            }
        }
    }
}
