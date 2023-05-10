using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace CGE.CleanCode.Common.Extensions
{
	public static class DatabaseCacheExtensions
	{
		public static async Task<T> GetRecordAsync<T>(this IDatabase cache, string recordId)
		{
			var jsonData = await cache.StringGetAsync(recordId);

			if (jsonData.IsNull)
			{
				return default(T);
			}
			else
			{
				return JsonConvert.DeserializeObject<T>(jsonData);
			}

		}

		public static async Task SetRecordAsync<T>(this IDatabase cache,
													string recordId,
													T data,
													TimeSpan? absolulteExpireTime = null,
													TimeSpan? unusedExpireTime = null)
		{
			var options = new DistributedCacheEntryOptions();

			options.AbsoluteExpirationRelativeToNow = absolulteExpireTime ?? TimeSpan.FromSeconds(60);
			options.SlidingExpiration = unusedExpireTime;

			var jsonData = JsonConvert.SerializeObject(data);

			await cache.StringSetAsync(recordId, jsonData, expiry:absolulteExpireTime ?? TimeSpan.FromDays(180));
		}

		public static void RemoveRecordAsync<T>(this IDatabase cache, string recordId)
		{
			cache.KeyDelete(recordId);
		}
	}
}
