namespace Service.PasswordRecovery.Domain.Models
{
	public interface IRecoveryInfo
	{
		string Email { get; set; }

		string Hash { get; set; }
	}
}