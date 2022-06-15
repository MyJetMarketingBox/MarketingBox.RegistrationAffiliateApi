using Autofac;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Auth.Service.Client;
using MarketingBox.Email.Service.Domain.Models;
using MyJetWallet.Sdk.NoSql;

namespace Service.MarketingBox.RegistrationAffiliateApi.Modules
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAffiliateServiceClient(Program.Settings.AffiliateServiceUrl);
            builder.RegisterAuthServiceClient(Program.Settings.AuthServiceUrl);
            
            builder.RegisterMyNoSqlWriter<AffiliateConfirmationNoSql>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                AffiliateConfirmationNoSql.TableName);
        }
    }
}