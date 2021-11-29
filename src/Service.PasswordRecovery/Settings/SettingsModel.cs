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
    }
}
