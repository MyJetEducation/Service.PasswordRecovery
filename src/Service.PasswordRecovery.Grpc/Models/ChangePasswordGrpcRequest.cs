using System.Runtime.Serialization;

namespace Service.PasswordRecovery.Grpc.Models
{
	[DataContract]
	public class ChangePasswordGrpcRequest
	{
		[DataMember(Order = 1)]
		public string Password { get; set; }

		[DataMember(Order = 2)]
		public string Hash { get; set; }
	}
}