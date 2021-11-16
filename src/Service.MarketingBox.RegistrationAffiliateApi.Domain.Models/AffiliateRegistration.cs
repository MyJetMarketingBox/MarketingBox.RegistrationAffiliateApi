using System;

namespace Service.MarketingBox.RegistrationAffiliateApi.Domain.Models
{
    public class AffiliateRegistration
    {
        public DateTime RequestDate { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}