using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.Core.Client.Extensions;
using Service.Core.Client.Models;
using Service.Core.Client.Services;
using Service.Grpc;
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
		private readonly IGrpcServiceProxy<IUserInfoService> _userInfoService;
		private readonly IEncoderDecoder _encoderDecoder;
		private readonly ISystemClock _systemClock;

		public PasswordRecoveryService(ILogger<PasswordRecoveryService> logger,
			IServiceBusPublisher<RecoveryInfoServiceBusModel> publisher,
			IGrpcServiceProxy<IUserInfoService> userInfoService, IEncoderDecoder encoderDecoder, ISystemClock systemClock)
		{
			_logger = logger;
			_publisher = publisher;
			_userInfoService = userInfoService;
			_encoderDecoder = encoderDecoder;
			_systemClock = systemClock;
		}

		public async ValueTask<CommonGrpcResponse> Recovery(RecoveryPasswordGrpcRequest request)
		{
			var recoveryInfoServiceBusModel = new RecoveryInfoServiceBusModel
			{
				Email = request.Email,
				Hash = GeneratePasswordRecoveryToken(request.Email)
			};

			_logger.LogDebug($"Publish password recovery info to service bus: {JsonSerializer.Serialize(recoveryInfoServiceBusModel)}");

			await _publisher.PublishAsync(recoveryInfoServiceBusModel);

			return CommonGrpcResponse.Success;
		}

		private string GeneratePasswordRecoveryToken(string userName)
		{
			int timeout = Program.ReloadedSettings(model => model.PasswordRecoveryTokenExpireMinutes).Invoke();

			return _encoderDecoder.EncodeProto(new PasswordRecoveryTokenInfo
			{
				PasswordRecoveryEmail = userName,
				PasswordRecoveryExpires = _systemClock.Now.AddMinutes(timeout)
			});
		}

		public async ValueTask<CommonGrpcResponse> Change(ChangePasswordGrpcRequest request)
		{
			string password = request.Password;
			string token = request.Hash;

			PasswordRecoveryTokenInfo tokenInfo = DecodePasswordRecoveryToken(token);
			if (tokenInfo == null)
				return CommonGrpcResponse.Fail;

			string email = tokenInfo.PasswordRecoveryEmail;
			string emailMasked = email.Mask();

			if (tokenInfo.PasswordRecoveryExpires < _systemClock.Now)
			{
				_logger.LogWarning("Token {token} for user: {user} has expired ({date})", token, emailMasked, tokenInfo.PasswordRecoveryExpires);
				return CommonGrpcResponse.Fail;
			}

			_logger.LogDebug("Confirm user registration for user {user} with hash: {hash}.", emailMasked, token);

			if (email == null || password.IsNullOrWhiteSpace())
				return CommonGrpcResponse.Fail;

			_logger.LogDebug("Changing password for {email} with new password.", emailMasked);

			CommonGrpcResponse response = await _userInfoService.TryCall(service => service.ChangePasswordAsync(new UserInfoChangePasswordRequest
			{
				UserName = email,
				Password = password
			}));

			return CommonGrpcResponse.Result(response.IsSuccess);
		}

		private PasswordRecoveryTokenInfo DecodePasswordRecoveryToken(string token)
		{
			PasswordRecoveryTokenInfo tokenInfo = null;

			try
			{
				tokenInfo = _encoderDecoder.DecodeProto<PasswordRecoveryTokenInfo>(token);
			}
			catch (Exception exception)
			{
				_logger.LogError("Can't decode registration token info ({token}), with message {message}", token, exception.Message);
			}

			return tokenInfo;
		}

	}
}