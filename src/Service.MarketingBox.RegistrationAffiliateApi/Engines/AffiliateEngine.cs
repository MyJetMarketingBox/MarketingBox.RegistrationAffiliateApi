using System;
using System.Net;
using MarketingBox.Affiliate.Service.Grpc;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service.MarketingBox.RegistrationAffiliateApi.Engines
{
    public class AffiliateEngine
    {
        private readonly IAffiliateService _affiliateService;

        public AffiliateEngine(IAffiliateService affiliateService)
        {
            _affiliateService = affiliateService;
        }
    }
}