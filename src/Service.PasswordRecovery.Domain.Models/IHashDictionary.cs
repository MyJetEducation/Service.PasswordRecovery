namespace Service.PasswordRecovery.Domain.Models
{
	public interface IHashDictionary
	{
		string GetEmail(string hash);

		string NewHash(string email);
	}
}