using System.ServiceModel;
using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.PasswordRecovery.Grpc.Models;

namespace Service.PasswordRecovery.Grpc
{
	[ServiceContract]
	public interface IPasswordRecoveryService
	{
		[OperationContract]
		ValueTask<CommonGrpcResponse> Recovery(RecoveryPasswordGrpcRequest request);

		[OperationContract]
		ValueTask<CommonGrpcResponse> Change(ChangePasswordGrpcRequest request);
	}
}