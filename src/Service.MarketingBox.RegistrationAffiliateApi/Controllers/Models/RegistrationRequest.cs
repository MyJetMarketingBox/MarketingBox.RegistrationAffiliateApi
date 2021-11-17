namespace Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models
{
    public class RegistrationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string LandingUrl { get; set; }
        public SubEntity[] Sub { get; set; }
    }

    public class SubEntity
    {
        public string SubName { get; set; }
        public string SubValue { get; set; }
    }
}