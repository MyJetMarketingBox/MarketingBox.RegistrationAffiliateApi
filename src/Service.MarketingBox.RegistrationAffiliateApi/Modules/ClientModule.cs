using Autofac;
using MarketingBox.Affiliate.Service.Client;
using MyJetWallet.Sdk.NoSql;
using Service.MarketingBox.Email.Service.Domain.Models;

namespace Service.MarketingBox.RegistrationAffiliateApi.Modules
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAffiliateServiceClient(Program.Settings.AffiliateServiceUrl);
            
            builder.RegisterMyNoSqlWriter<AffiliateConfirmationNoSql>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                AffiliateConfirmationNoSql.TableName);
        }
    }
}