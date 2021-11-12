using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Service.MarketingBox.RegistrationAffiliateApi.Engines;

namespace Service.MarketingBox.RegistrationAffiliateApi.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<AffiliateEngine>()
                .AsSelf()
                .SingleInstance();
        }
    }
}