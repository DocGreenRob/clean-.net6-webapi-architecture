using CGE.CleanCode.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CGE.CleanCode.Service.Interfaces
{
	public interface ICacheService
	{
		Task AddEntityToListCacheAsync<T>(string entityType, T entityDto);
		Task<T> GetRecordFromCacheAsync<T>(string cacheKey);
		void RemoveAllKeys();
		Task RemoveEntityFromListNameAndIdCacheAsync(string entityType, string id);
		void RemoveKey(string key);
		Task SetRecordInCacheAsync<T>(string cacheKey, T record);
		Task<List<CacheEntry>> GetAllKeys();
	}
}