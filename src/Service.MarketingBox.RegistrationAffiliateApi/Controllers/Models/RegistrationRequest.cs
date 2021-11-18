using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests;

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
}