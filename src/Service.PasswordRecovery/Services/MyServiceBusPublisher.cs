using System.Threading.Tasks;
using DotNetCoreDecorators;
using MyServiceBus.TcpClient;
using Service.PasswordRecovery.Domain.Models;
using SimpleTrading.ServiceBus.CommonUtils.Serializers;

namespace Service.PasswordRecovery.Services
{
	public class MyServiceBusPublisher : IPublisher<RecoveryInfoServiceBusModel>
	{
		private readonly MyServiceBusTcpClient _client;

		public MyServiceBusPublisher(MyServiceBusTcpClient client)
		{
			_client = client;
			_client.CreateTopicIfNotExists(RecoveryInfoServiceBusModel.TopicName);
		}

		public ValueTask PublishAsync(RecoveryInfoServiceBusModel valueToPublish)
		{
			byte[] bytesToSend = valueToPublish.ServiceBusContractToByteArray();

			Task task = _client.PublishAsync(RecoveryInfoServiceBusModel.TopicName, bytesToSend, false);

			return new ValueTask(task);
		}
	}
}