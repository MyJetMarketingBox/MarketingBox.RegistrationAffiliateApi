using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service.MarketingBox.RegistrationAffiliateApi.Engines
{
    public class EmailEngine
    {
        private const string FromAddress = "somemail@gmail.com";
        private const string FromName = "Tom";
        private const string EmailLogin = "somemail@gmail.com";
        private const string EmailPassword = "pass";
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;

        public async Task SendEmailAsync(string subject, string body)
        {
            var from = new MailAddress(FromAddress, FromName);
            var to = new MailAddress(EmailLogin);
            var m = new MailMessage(from, to)
            {
                Subject = subject, 
                Body = body
            };
            var smtp = new SmtpClient("smtp.gmail.com", SmtpPort)
            {
                Credentials = new NetworkCredential(EmailLogin, EmailPassword), 
                EnableSsl = true
            };
            await smtp.SendMailAsync(m);
        }
    }
}