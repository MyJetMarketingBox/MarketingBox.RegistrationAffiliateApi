using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.MarketingBox.RegistrationAffiliateApi.Domain.Models;
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
            builder
                .RegisterType<EmailEngine>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterMyNoSqlWriter<AffiliateRegistrationNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), 
                AffiliateRegistrationNoSql.TableName);
        }
    }
}