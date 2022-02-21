using Autofac;
using Microsoft.Extensions.Logging;
using Service.Grpc;
using Service.PasswordRecovery.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.PasswordRecovery.Client
{
	public static class AutofacHelper
	{
		public static void RegisterPasswordRecoveryClient(this ContainerBuilder builder, string grpcServiceUrl, ILogger logger)
		{
			var factory = new PasswordRecoveryClientFactory(grpcServiceUrl, logger);

			builder.RegisterInstance(factory.GetPasswordRecoveryService()).As<IGrpcServiceProxy<IPasswordRecoveryService>>().SingleInstance();
		}
	}
}