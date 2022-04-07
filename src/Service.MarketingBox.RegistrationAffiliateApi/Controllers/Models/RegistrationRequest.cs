using System.ComponentModel.DataAnnotations;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;

namespace Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models
{
    public class RegistrationRequest
    {
        [Required]
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public SubEntity[] Sub { get; set; }
    }
}