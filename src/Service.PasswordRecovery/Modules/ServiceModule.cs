using Autofac;
using DotNetCoreDecorators;
using MyServiceBus.TcpClient;
using Service.Core.Client.Services;
using Service.PasswordRecovery.Domain.Models;
using Service.PasswordRecovery.Models;
using Service.PasswordRecovery.Services;
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

			var tcpServiceBus = new MyServiceBusTcpClient(() => Program.Settings.ServiceBusWriter, "MyJetEducation Service.PasswordRecovery");
			IPublisher<RecoveryInfoServiceBusModel> clientRegisterPublisher = new MyServiceBusPublisher(tcpServiceBus);
			builder.Register(context => clientRegisterPublisher);
			tcpServiceBus.Start();
		}
	}
}