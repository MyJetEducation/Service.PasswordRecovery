using System.Runtime.Serialization;
using Service.PasswordRecovery.Domain.Models;

namespace Service.PasswordRecovery.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}