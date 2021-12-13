using System.Text.Json;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.PasswordRecovery.Domain.Models;
using Service.PasswordRecovery.Grpc;
using Service.PasswordRecovery.Grpc.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;
using CommonGrpcResponse = Service.PasswordRecovery.Grpc.Models.CommonGrpcResponse;

namespace Service.PasswordRecovery.Services
{
	public class PasswordRecoveryService : IPasswordRecoveryService
	{
		private readonly ILogger<PasswordRecoveryService> _logger;
		private readonly IPublisher<IRecoveryInfo> _publisher;
		private readonly IHashDictionary _hashDictionary;
		private readonly IUserInfoService _userInfoService;

		public PasswordRecoveryService(ILogger<PasswordRecoveryService> logger,
			IPublisher<IRecoveryInfo> publisher,
			IHashDictionary hashDictionary,
			IUserInfoService userInfoService)
		{
			_logger = logger;
			_publisher = publisher;
			_hashDictionary = hashDictionary;
			_userInfoService = userInfoService;
		}

		public async ValueTask<CommonGrpcResponse> Recovery(RecoveryPasswordGrpcRequest request)
		{
			string email = request.Email;
			string hash = _hashDictionary.NewHash(email);

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
			string hash = request.Hash;
			string email = _hashDictionary.GetEmail(hash);
			string password = request.Password;

			if (email == null || string.IsNullOrWhiteSpace(password))
				return CommonGrpcResponse.Fail;

			_logger.LogDebug($"Changing password for {email} with {password}.");

			UserInfo.Crud.Grpc.Models.CommonGrpcResponse response = await _userInfoService.ChangePasswordAsync(new UserInfoChangePasswordRequest
			{
				Email = email, 
				Password = password
			});

			return CommonGrpcResponse.Result(response.IsSuccess);
		}
	}
}