using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.MarketingBox.RegistrationAffiliateApi.Settings
{
    public class SettingsModel
    {
        [YamlProperty("RegistrationAffiliateApi.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("RegistrationAffiliateApi.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("RegistrationAffiliateApi.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("RegistrationAffiliateApi.AffiliateServiceUrl")]
        public string AffiliateServiceUrl { get; set; }
    }
}
