using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CGE.CleanCode.Common.Extensions
{
	public static class DistributedCacheExtensions
	{
		public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
		{
			var jsonData = await cache.GetStringAsync(recordId);

			if (jsonData is null)
			{
				return default(T);
			}
			else
			{
				return JsonConvert.DeserializeObject<T>(jsonData);
			}

		}

		public static async Task SetRecordAsync<T>(this IDistributedCache cache,
															string recordId,
													T data,
													TimeSpan? absolulteExpireTime = null,
													TimeSpan? unusedExpireTime = null)
		{
			var options = new DistributedCacheEntryOptions();

			options.AbsoluteExpirationRelativeToNow = absolulteExpireTime ?? TimeSpan.FromSeconds(60);
			options.SlidingExpiration = unusedExpireTime;

			var jsonData = JsonConvert.SerializeObject(data);

			await cache.SetStringAsync(recordId, jsonData, options);
		}
	}
}
