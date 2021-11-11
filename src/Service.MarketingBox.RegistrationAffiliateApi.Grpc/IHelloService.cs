using System.ServiceModel;
using System.Threading.Tasks;
using Service.MarketingBox.RegistrationAffiliateApi.Grpc.Models;

namespace Service.MarketingBox.RegistrationAffiliateApi.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}