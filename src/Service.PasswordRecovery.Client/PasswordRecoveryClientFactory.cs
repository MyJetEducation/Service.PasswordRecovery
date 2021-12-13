using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.PasswordRecovery.Grpc;

namespace Service.PasswordRecovery.Client
{
    [UsedImplicitly]
    public class PasswordRecoveryClientFactory: MyGrpcClientFactory
    {
        public PasswordRecoveryClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IPasswordRecoveryService GetPasswordRecoveryService() => CreateGrpcService<IPasswordRecoveryService>();
    }
}
