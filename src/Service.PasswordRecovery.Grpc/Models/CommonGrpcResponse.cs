using System.Runtime.Serialization;

namespace Service.PasswordRecovery.Grpc.Models
{
	[DataContract]
	public class CommonGrpcResponse
	{
		[DataMember(Order = 1)]
		public bool IsSuccess { get; set; }

		public static CommonGrpcResponse Success => new() {IsSuccess = true};
		public static CommonGrpcResponse Fail => new();
		public static CommonGrpcResponse Result(bool result) => new() {IsSuccess = result};
	}
}