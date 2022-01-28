using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.TcpClient;
using Service.Core.Client.Services;
using Service.PasswordRecovery.Models;
using Service.PasswordRecovery.Services;
using Service.ServiceBus.Models;
using Service.UserInfo.Crud.Client;

namespace Service.PasswordRecovery.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterUserInfoCrudClient(Program.Settings.UserInfoCrudServiceUrl);
			builder.RegisterType<PasswordRecoveryService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<SystemClock>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<HashCodeService<EmailHashDto>>().As<IHashCodeService<EmailHashDto>>().SingleInstance();

			MyServiceBusTcpClient tcpServiceBus = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.ServiceBusWriter), Program.LogFactory);
			builder.RegisterMyServiceBusPublisher<RecoveryInfoServiceBusModel>(tcpServiceBus, RecoveryInfoServiceBusModel.TopicName, false);
		}
	}
}