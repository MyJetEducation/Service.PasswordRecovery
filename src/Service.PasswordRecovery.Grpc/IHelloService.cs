using System.ServiceModel;
using System.Threading.Tasks;
using Service.PasswordRecovery.Grpc.Models;

namespace Service.PasswordRecovery.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}