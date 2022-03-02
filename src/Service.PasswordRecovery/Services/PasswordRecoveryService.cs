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
		private readonly IObjectCache<string> _userNameHashCache;

		public PasswordRecoveryService(ILogger<PasswordRecoveryService> logger,
			IServiceBusPublisher<RecoveryInfoServiceBusModel> publisher,
			IGrpcServiceProxy<IUserInfoService> userInfoService, 
			IEncoderDecoder encoderDecoder, 
			ISystemClock systemClock, 
			IObjectCache<string> userNameHashCache)
		{
			_logger = logger;
			_publisher = publisher;
			_userInfoService = userInfoService;
			_encoderDecoder = encoderDecoder;
			_systemClock = systemClock;
			_userNameHashCache = userNameHashCache;
		}

		public async ValueTask<CommonGrpcResponse> Recovery(RecoveryPasswordGrpcRequest request)
		{
			string userName = request.Email;

			if (_userNameHashCache.Exists(userName))
				return CommonGrpcResponse.Success;

			int timeout = Program.ReloadedSettings(model => model.PasswordRecoveryTokenExpireMinutes).Invoke();
			DateTime expires = _systemClock.Now.AddMinutes(timeout);

			var recoveryInfoServiceBusModel = new RecoveryInfoServiceBusModel
			{
				Email = userName,
				Hash = _encoderDecoder.EncodeProto(new PasswordRecoveryTokenInfo
				{
					PasswordRecoveryEmail = userName,
					PasswordRecoveryExpires = expires
				})
			};

			_logger.LogDebug($"Publish password recovery info to service bus: {JsonSerializer.Serialize(recoveryInfoServiceBusModel)}");

			await _publisher.PublishAsync(recoveryInfoServiceBusModel);

			_userNameHashCache.Add(userName, expires);

			return CommonGrpcResponse.Success;
		}

		public async ValueTask<CommonGrpcResponse> Change(ChangePasswordGrpcRequest request)
		{
			string token = request.Hash;

			if (_userNameHashCache.Exists(token))
				return CommonGrpcResponse.Fail;

			PasswordRecoveryTokenInfo tokenInfo = DecodePasswordRecoveryToken(token);
			if (tokenInfo == null)
				return CommonGrpcResponse.Fail;

			string email = tokenInfo.PasswordRecoveryEmail;
			string emailMasked = email.Mask();

			DateTime expires = tokenInfo.PasswordRecoveryExpires;
			if (expires < _systemClock.Now)
			{
				_logger.LogWarning("Token {token} for user: {user} has expired ({date})", token, emailMasked, expires);
				return CommonGrpcResponse.Fail;
			}

			_logger.LogDebug("Confirm user registration for user {user} with hash: {hash}.", emailMasked, token);

			string password = request.Password;
			if (email == null || password.IsNullOrWhiteSpace())
				return CommonGrpcResponse.Fail;

			_logger.LogDebug("Changing password for {email} with new password.", emailMasked);

			CommonGrpcResponse response = await _userInfoService.TryCall(service => service.ChangePasswordAsync(new UserInfoChangePasswordRequest
			{
				UserName = email,
				Password = password
			}));

			_userNameHashCache.Add(token, expires);

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