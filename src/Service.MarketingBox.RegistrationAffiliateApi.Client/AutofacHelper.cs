using Autofac;
using Service.MarketingBox.RegistrationAffiliateApi.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.MarketingBox.RegistrationAffiliateApi.Client
{
    public static class AutofacHelper
    {
        public static void RegistrationAffiliateApiClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new RegistrationAffiliateApiClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
