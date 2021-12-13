using System.Runtime.Serialization;

namespace Service.PasswordRecovery.Grpc.Models
{
    [DataContract]
    public class RecoveryPasswordGrpcRequest
    {
        [DataMember(Order = 1)]
        public string Email { get; set; }
    }
}