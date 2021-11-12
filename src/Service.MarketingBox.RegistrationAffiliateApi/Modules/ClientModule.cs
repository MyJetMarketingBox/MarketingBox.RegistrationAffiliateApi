using Autofac;
using MarketingBox.Affiliate.Service.Client;

namespace Service.MarketingBox.RegistrationAffiliateApi.Modules
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAffiliateServiceClient(Program.Settings.AffiliateServiceUrl);
        }
    }
}