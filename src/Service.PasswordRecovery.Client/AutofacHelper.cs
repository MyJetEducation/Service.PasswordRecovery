using Autofac;
using Service.PasswordRecovery.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.PasswordRecovery.Client
{
    public static class AutofacHelper
    {
        public static void RegisterPasswordRecoveryClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new PasswordRecoveryClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
