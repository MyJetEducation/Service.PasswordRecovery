using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Service.PasswordRecovery.Domain.Models
{
	public class HashDictionary : IHashDictionary
	{
		private const int HashLiveTimeMinutes = 30;

		private static readonly ConcurrentDictionary<string, HashInfo> Dictionary;

		static HashDictionary() => Dictionary = new ConcurrentDictionary<string, HashInfo>();

		public string GetEmail(string hash)
		{
			CheckHash();

			if (string.IsNullOrWhiteSpace(hash))
				return null;

			return Dictionary.TryGetValue(hash, out HashInfo info)
				? info.Email
				: null;
		}

		public string NewHash(string email)
		{
			CheckHash();

			string hash = GenerateHash();

			Dictionary.TryAdd(hash, new HashInfo(email));

			return hash;
		}

		private static void CheckHash()
		{
			KeyValuePair<string, HashInfo>[] pairs = Dictionary
				.Where(pair => pair.Value.IsExpired)
				.ToArray();

			foreach (KeyValuePair<string, HashInfo> pair in pairs)
				Dictionary.TryRemove(pair);
		}

		private static DateTime GetTime() => DateTime.UtcNow;

		private static string GenerateHash() => Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");

		private class HashInfo
		{
			public HashInfo(string email)
			{
				Created = GetTime();
				Email = email;
			}

			private DateTime Created { get; }
			public string Email { get; }

			public bool IsExpired => Created.AddMinutes(HashLiveTimeMinutes) > GetTime();
		}
	}
}