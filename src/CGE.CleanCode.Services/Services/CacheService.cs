using Cge.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CGE.CleanCode.Common;
using CGE.CleanCode.Common.Extensions;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Service.Interfaces;
using CGE.CleanCode.Service.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CGE.CleanCode.Service.Services
{
	public class CacheService : ICacheService
	{
		private readonly IConfiguration _configuration;
		private readonly string _redisCacheHostName;
		private readonly string _redisCacheKey;
		private readonly string _cacheReplaceSpaceString;
		private IDatabase _cache = null;
		private Lazy<ConnectionMultiplexer> _lazyConnection;
		private readonly ICacheConfigruationMangementService _cacheConfigruationMangementService;

		public CacheService(IConfiguration configuration, ICacheConfigruationMangementService cacheConfigruationMangementService)
		{
			// Validate stuff
			_configuration = configuration.ValidateArgNotNull(nameof(configuration));
			_configuration[Globals.ConfigurationKeys.RedisCacheHostName].ValidateArgNotNull(Globals.ConfigurationKeys.RedisCacheHostName);
			_configuration[Globals.ConfigurationKeys.RedisCacheKey].ValidateArgNotNull(Globals.ConfigurationKeys.RedisCacheKey);
			_configuration[Globals.ConfigurationKeys.CacheReplaceSpaceString].ValidateArgNotNull(Globals.ConfigurationKeys.CacheReplaceSpaceString);

			// Set values from configuration
			_redisCacheHostName = _configuration[Globals.ConfigurationKeys.RedisCacheHostName];
			_redisCacheKey = _configuration[Globals.ConfigurationKeys.RedisCacheKey];
			_cacheReplaceSpaceString = _configuration[Globals.ConfigurationKeys.CacheReplaceSpaceString];
			_cacheConfigruationMangementService = cacheConfigruationMangementService.ValidateArgNotNull(nameof(cacheConfigruationMangementService));

			// Connect to AzureRedis
			_lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
			{
				string cacheConnection = $"{_redisCacheHostName},password={_redisCacheKey},ssl=true,abortConnect=false,allowAdmin=True,connectRetry=5";
				return ConnectionMultiplexer.Connect(cacheConnection);
			});

			_cache = _lazyConnection.Value.GetDatabase();
		}

		public async Task AddEntityToListCacheAsync<T>(string entityType, T entityDto)
		{
			var cacheKey = string.Empty;

			// remove from list
			// TODO: update and get keys from _cacheConfigruationMangementService
			switch (entityType.ToLower())
			{
				
				case "athlete":
					cacheKey = Globals.CacheKeys.AthleteList;
					break;
				
			}

			var list = await GetListFromCacheAsync<T>(cacheKey);
			if (list == null)
			{
				list = new List<T>();
			}
			list.Add(entityDto);

			await _cache.SetRecordAsync(cacheKey, list);
		}

		public async Task<T> GetRecordFromCacheAsync<T>(string cacheKey)
		{
			return await _cache.GetRecordAsync<T>(cacheKey);
		}

		public void RemoveAllKeys()
		{
			var endpoints = _lazyConnection.Value.GetEndPoints();
			var server = _lazyConnection.Value.GetServer(endpoints.First());

			foreach (var key in server.Keys())
			{
				_cache.KeyDelete(key);
				_cache.KeyExpire(key, TimeSpan.FromSeconds(0));
			}
		}

		public async Task<List<CacheEntry>> GetAllKeys()
		{
			var endpoints = _lazyConnection.Value.GetEndPoints();
			var server = _lazyConnection.Value.GetServer(endpoints.First());
			var w = new List<CacheEntry>();

			foreach (var key in server.Keys())
			{
				var t = _cache.GetType();
				var x = await _cache.GetRecordAsync<dynamic>(key);
				var y = new CacheEntry
				{
					Key = key,
					Value = JsonConvert.SerializeObject(x)
				};
				w.Add(y);
			}

			return w;
		}

		public async Task RemoveEntityFromListNameAndIdCacheAsync(string entityType, string id)
		{
			var cacheKey = string.Empty;

			// remove from list
			switch (entityType.ToLower())
			{
				case "athlete":
					//cacheKey = _cacheConfigruationMangementService.GetListKey<AthleteDto>();
					//var athleteList = await GetListFromCacheAsync<AthleteDto>(cacheKey);
					//var athleteDto = athleteList.Where(x => x.Id == id).FirstOrDefault();

					//if (athleteDto == null || athleteDto == default(AthleteDto))
					//{
					//	return;
					//}

					//var newAthleteList = athleteList.Where(x => x.Id != id).ToList();

					//// Recycle the list
					//_cache.KeyDelete(cacheKey);
					//await _cache.SetRecordAsync(cacheKey, newAthleteList);

					//// Delete custom key
					//cacheKey = _cacheConfigruationMangementService.GetCustomKey<AthleteDto>(athleteDto);
					//_cache.KeyDelete(cacheKey);

					//// Delete Id based key	
					//cacheKey = _cacheConfigruationMangementService.GetKeyById<AthleteDto>(athleteDto);
					//_cache.KeyDelete(cacheKey);

					break;
				
			}
		}

		public void RemoveKey(string key)
		{
			_cache.KeyDelete(key);
		}

		public async Task SetRecordInCacheAsync<T>(string cacheKey, T record)
		{
			RemoveKey(cacheKey);
			await _cache.SetRecordAsync<T>(cacheKey, record);
		}

		private async Task<List<T>> GetListFromCacheAsync<T>(string cacheKey)
		{
			return await _cache.GetRecordAsync<List<T>>(cacheKey);
		}
	}
}
