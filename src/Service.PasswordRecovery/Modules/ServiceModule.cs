using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.TcpClient;
using Service.Core.Client.Services;
using Service.PasswordRecovery.Services;
using Service.ServiceBus.Models;
using Service.UserInfo.Crud.Client;

namespace Service.PasswordRecovery.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterUserInfoCrudClient(Program.Settings.UserInfoCrudServiceUrl, Program.LogFactory.CreateLogger(typeof(UserInfoCrudClientFactory)));

			builder.RegisterType<PasswordRecoveryService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<SystemClock>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<ObjectCache<string>>().AsImplementedInterfaces().SingleInstance();

			builder.Register(context => new EncoderDecoder(Program.EncodingKey))
				.As<IEncoderDecoder>()
				.SingleInstance();

			var tcpServiceBus = new MyServiceBusTcpClient(() => Program.Settings.ServiceBusWriter, "MyJetEducation Service.PasswordRecovery");

			builder
				.Register(context => new MyServiceBusPublisher<RecoveryInfoServiceBusModel>(tcpServiceBus, RecoveryInfoServiceBusModel.TopicName, false))
				.As<IServiceBusPublisher<RecoveryInfoServiceBusModel>>()
				.SingleInstance();

			tcpServiceBus.Start();
		}
	}
}