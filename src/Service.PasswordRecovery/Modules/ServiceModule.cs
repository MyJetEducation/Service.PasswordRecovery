using Autofac;
using Service.PasswordRecovery.Domain.Models;
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
			builder.RegisterType<HashDictionary>().AsImplementedInterfaces().SingleInstance();
		}
	}
}