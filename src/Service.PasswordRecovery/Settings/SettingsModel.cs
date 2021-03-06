using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.PasswordRecovery.Settings
{
	public class SettingsModel
	{
		[YamlProperty("PasswordRecovery.SeqServiceUrl")]
		public string SeqServiceUrl { get; set; }

		[YamlProperty("PasswordRecovery.ZipkinUrl")]
		public string ZipkinUrl { get; set; }

		[YamlProperty("PasswordRecovery.ElkLogs")]
		public LogElkSettings ElkLogs { get; set; }

		[YamlProperty("PasswordRecovery.ServiceBusWriter")]
		public string ServiceBusWriter { get; set; }

		[YamlProperty("PasswordRecovery.UserInfoCrudServiceUrl")]
		public string UserInfoCrudServiceUrl { get; set; }

		[YamlProperty("PasswordRecovery.PasswordRecoveryTokenExpireMinutes")]
		public int PasswordRecoveryTokenExpireMinutes { get; set; }

	}
}