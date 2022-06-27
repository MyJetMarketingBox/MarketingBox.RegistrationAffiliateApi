using System.ComponentModel.DataAnnotations;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Sdk.Common.Models;

namespace Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models
{
    public class RegistrationRequest : ValidatableEntity
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Phone { get; set; }
        public SubEntity[] Sub { get; set; }
    }
}