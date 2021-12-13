using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.PasswordRecovery.Client;
using Service.PasswordRecovery.Grpc;
using Service.PasswordRecovery.Grpc.Models;

namespace TestApp
{
	internal class Program
	{
		private static async Task Main()
		{
			GrpcClientFactory.AllowUnencryptedHttp2 = true;

			Console.Write("Press enter to start");
			Console.ReadLine();

			var factory = new PasswordRecoveryClientFactory("http://localhost:5001");
			IPasswordRecoveryService client = factory.GetPasswordRecoveryService();

			await client.Recovery(new RecoveryPasswordGrpcRequest {Email = "some@email.com"});

			Console.WriteLine("End");
			Console.ReadLine();
		}
	}
}