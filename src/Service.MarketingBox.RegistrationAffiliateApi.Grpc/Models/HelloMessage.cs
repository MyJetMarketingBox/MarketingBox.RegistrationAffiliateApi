using System.Runtime.Serialization;
using Service.MarketingBox.RegistrationAffiliateApi.Domain.Models;

namespace Service.MarketingBox.RegistrationAffiliateApi.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}