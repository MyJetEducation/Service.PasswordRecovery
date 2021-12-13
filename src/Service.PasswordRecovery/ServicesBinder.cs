using DotNetCoreDecorators;
using MyDependencies;
using MyServiceBus.TcpClient;
using Service.PasswordRecovery.Domain.Models;
using Service.PasswordRecovery.Services;
using Service.PasswordRecovery.Settings;

namespace Service.PasswordRecovery
{
	public static class ServicesBinder
	{
		public static MyServiceBusTcpClient BindServiceBus(this IServiceRegistrator sr, SettingsModel settingsModel)
		{
			var tcpServiceBus = new MyServiceBusTcpClient(() => settingsModel.ServiceBusWriter, Program.AppName);

			var clientRegisterPublisher = new MyServiceBusPublisher(tcpServiceBus);

			sr.Register<IPublisher<IRecoveryInfo>>(clientRegisterPublisher);

			return tcpServiceBus;
		}
	}
}