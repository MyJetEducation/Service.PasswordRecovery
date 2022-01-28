using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.Core.Client.Services;
using Service.PasswordRecovery.Grpc;
using Service.PasswordRecovery.Grpc.Models;
using Service.PasswordRecovery.Models;
using Service.ServiceBus.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;

namespace Service.PasswordRecovery.Services
{
	public class PasswordRecoveryService : IPasswordRecoveryService
	{
		private readonly ILogger<PasswordRecoveryService> _logger;
		private readonly IServiceBusPublisher<RecoveryInfoServiceBusModel> _publisher;
		private readonly IHashCodeService<EmailHashDto> _hashCodeService;
		private readonly IUserInfoService _userInfoService;

		public PasswordRecoveryService(ILogger<PasswordRecoveryService> logger,
			IServiceBusPublisher<RecoveryInfoServiceBusModel> publisher,
			IHashCodeService<EmailHashDto> hashCodeService,
			IUserInfoService userInfoService)
		{
			_logger = logger;
			_publisher = publisher;
			_hashCodeService = hashCodeService;
			_userInfoService = userInfoService;
		}

		public async ValueTask<CommonGrpcResponse> Recovery(RecoveryPasswordGrpcRequest request)
		{
			string email = request.Email;
			string hash = _hashCodeService.New(new EmailHashDto(email));

			if (hash == null)
				return CommonGrpcResponse.Fail;

			var recoveryInfoServiceBusModel = new RecoveryInfoServiceBusModel
			{
				Email = email,
				Hash = hash
			};

			_logger.LogDebug($"Publish into to service bus: {JsonSerializer.Serialize(recoveryInfoServiceBusModel)}");

			await _publisher.PublishAsync(recoveryInfoServiceBusModel);

			return CommonGrpcResponse.Success;
		}

		public async ValueTask<CommonGrpcResponse> Change(ChangePasswordGrpcRequest request)
		{
			string password = request.Password;
			string hash = request.Hash;
			string email = _hashCodeService.Get(hash)?.Email;

			if (email == null || password.IsNullOrWhiteSpace())
				return CommonGrpcResponse.Fail;

			_logger.LogDebug("Changing password for {email} with new password.", email);

			CommonGrpcResponse response = await _userInfoService.ChangePasswordAsync(new UserInfoChangePasswordRequest
			{
				UserName = email,
				Password = password
			});

			return CommonGrpcResponse.Result(response.IsSuccess);
		}
	}
}