using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Service.Grpc;
using Service.PasswordRecovery.Grpc;

namespace Service.PasswordRecovery.Client
{
    [UsedImplicitly]
    public class PasswordRecoveryClientFactory: GrpcClientFactory
    {
        public PasswordRecoveryClientFactory(string grpcServiceUrl, ILogger logger) : base(grpcServiceUrl, logger)
        {
        }

        public IGrpcServiceProxy<IPasswordRecoveryService> GetPasswordRecoveryService() => CreateGrpcService<IPasswordRecoveryService>();
    }
}
