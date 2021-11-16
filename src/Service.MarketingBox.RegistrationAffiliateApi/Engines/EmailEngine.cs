using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service.MarketingBox.RegistrationAffiliateApi.Engines
{
    public class EmailEngine
    {
        private const string FromAddress = "somemail@gmail.com";
        private const string FromName = "Tom";
        private const string EmailPassword = "pass";
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;

        private const string RegSubject = "Registration confirmation";
        private const string RegBody = "Please follow the link to confirm your registration: ";

        public async Task SendRegEmailAsync(string email, string token)
        {
            
            Console.WriteLine($"REG BODY : {GetRegBody(token)}");
            return;
            
            var from = new MailAddress(FromAddress, FromName);
            var to = new MailAddress(email);
            var m = new MailMessage(from, to)
            {
                Subject = RegSubject, 
                Body = GetRegBody(token)
            };
            var smtp = new SmtpClient(SmtpHost, SmtpPort)
            {
                Credentials = new NetworkCredential(FromAddress, EmailPassword), 
                EnableSsl = true
            };
            await smtp.SendMailAsync(m);
        }

        private static string GetRegBody(string token)
        {
            return RegBody + GetConfirmLink(token);
        }

        private static string GetConfirmLink(string token)
        {
            return Program.Settings.RegistrationAffiliateApiUrl + $"/api/affiliate/confirmation/{token}";
        }
    }
}