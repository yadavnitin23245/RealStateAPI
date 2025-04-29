using Microsoft.Extensions.Options;
using RealState.BAL.Helpers;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using RealState.BAL.DTO;

namespace RealState.Email
{
    public class EmailHelper
    {
        private readonly IOptions<AppSettingsDTO> _appSettings;
       
        public EmailHelper(IOptions<AppSettingsDTO> appSettings)
        {
            _appSettings = appSettings;
         
        }

        public bool SendEmailVerifyRequest(string userEmail, string body, string Subject)
        {


            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("userspadez123@gmail.com");
            // mailMessage.From = new MailAddress("userspadez123@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));
            mailMessage.Subject = Subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;

            SmtpClient client = new SmtpClient();

            client.Credentials = new System.Net.NetworkCredential("userspadez123@gmail.com", "mdbfyovmfoevaaox");
            // client.Credentials = new System.Net.NetworkCredential("userspadez123@gmail.com", "mdbfyovmfoevaaox");
            client.Host = "smtp.gmail.com";
            client.Port = Convert.ToInt32(587);
            client.EnableSsl = true;

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

    }
}
