using System.Runtime.Serialization;

namespace Service.PasswordRecovery.Domain.Models
{
	[DataContract]
	public class RecoveryInfoServiceBusModel
	{
		public const string TopicName = "myjeteducation-recovery-password";

		[DataMember(Order = 1)]
		public string Email { get; set; }

		[DataMember(Order = 2)]
		public string Hash { get; set; }
	}
}