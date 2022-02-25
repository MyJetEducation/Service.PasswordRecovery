using System;
using System.Runtime.Serialization;

namespace Service.PasswordRecovery.Models
{
	[DataContract]
	public class PasswordRecoveryTokenInfo
	{
		[DataMember(Order = 1)]
		public string PasswordRecoveryEmail { get; set; }

		[DataMember(Order = 2)]
		public DateTime PasswordRecoveryExpires { get; set; }
	}
}