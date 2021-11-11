using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.MarketingBox.RegistrationAffiliateApi.Grpc;

namespace Service.MarketingBox.RegistrationAffiliateApi.Client
{
    [UsedImplicitly]
    public class RegistrationAffiliateApiClientFactory: MyGrpcClientFactory
    {
        public RegistrationAffiliateApiClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}
