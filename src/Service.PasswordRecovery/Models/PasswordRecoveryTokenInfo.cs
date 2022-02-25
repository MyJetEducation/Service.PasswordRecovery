using System;

namespace Service.PasswordRecovery.Models
{
	public class PasswordRecoveryTokenInfo
	{
		public string PasswordRecoveryEmail { get; set; }

		public DateTime PasswordRecoveryExpires { get; set; }
	}
}