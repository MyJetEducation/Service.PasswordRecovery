using System.Threading.Tasks;
using DotNetCoreDecorators;
using MyServiceBus.TcpClient;
using Service.PasswordRecovery.Domain.Models;
using SimpleTrading.ServiceBus.CommonUtils.Serializers;

namespace Service.PasswordRecovery.Services
{
	public class MyServiceBusPublisher : IPublisher<IRecoveryInfo>
	{
		private readonly MyServiceBusTcpClient _client;

		public MyServiceBusPublisher(MyServiceBusTcpClient client)
		{
			_client = client;
			_client.CreateTopicIfNotExists(RecoveryInfoServiceBusModel.TopicName);
		}

		public ValueTask PublishAsync(IRecoveryInfo valueToPublish)
		{
			var serviceBusModel = new RecoveryInfoServiceBusModel
			{
				Email = valueToPublish.Email,
				Hash = valueToPublish.Hash
			};

			byte[] bytesToSend = serviceBusModel.ServiceBusContractToByteArray();

			Task task = _client.PublishAsync(RecoveryInfoServiceBusModel.TopicName, bytesToSend, false);

			return new ValueTask(task);
		}
	}
}