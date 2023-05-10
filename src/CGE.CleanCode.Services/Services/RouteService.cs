using AutoMapper;
using Cge.Core.Extensions;
using Cge.Core.Logging;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using CGE.CleanCode.Common;
using CGE.CleanCode.Common.Extensions;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Common.Models.Patch;
using CGE.CleanCode.Dal.Entities;
using CGE.CleanCode.Dal.Extensions;
using CGE.CleanCode.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CGE.CleanCode.Service.Services
{
    public class RouteService : BaseService<Route>, IRouteService
    {
        private readonly string _cacheReplaceSpaceString;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ICacheConfigruationMangementService _cacheConfigruationMangementService;

        public RouteService(IMapper mapper,
                                IConfiguration configuration,
                                ICacheService cacheService,
                                ITelemetryClient telemetryClient,
                                IMongoDbWrapper mongoDbWrapper,
                                ICacheConfigruationMangementService cacheConfigruationMangementService) : base(mongoDbWrapper,
                                                                                                            configuration,
                                                                                                            telemetryClient,
                                                                                                            Globals.Documents.Route)
        {
            // Validate stuff
            _cacheService = cacheService.ValidateArgNotNull(nameof(cacheService));
            _mapper = mapper.ValidateArgNotNull(nameof(mapper));
            _cacheConfigruationMangementService = cacheConfigruationMangementService.ValidateArgNotNull(nameof(cacheConfigruationMangementService));
        }

        public async Task DeleteAsync(string id)
        {
            var result = await DatabaseAdapter.GetAsync<Route>(id: id);

            if (result == null || result == default(Route))
            {
                throw new Exception();
            }

            result.IsDeleted = true;
            result.UpdatedDate = DateTime.Now;
            result.UpdatedBy = "updated-by-user";

            await DatabaseAdapter.UpsertAsync(id, result);
        }

        public async Task<IEnumerable<RouteDto>> GetAllAsync()
        {
            var cacheKey = _cacheConfigruationMangementService.GetListKey<RouteDto>();
            var cachedValue = _cacheService.GetRecordFromCacheAsync<List<RouteDto>>(cacheKey).Result;

            if (cachedValue is null || cachedValue is default(List<RouteDto>))
            {
                var results = await DatabaseAdapter.GetAllAsync<Route>(x => !x.IsDeleted);
                var routeDtos = results.Select(x => _mapper.Map<RouteDto>(x)).AsEnumerable();

                await _cacheService.SetRecordInCacheAsync<List<RouteDto>>(cacheKey, routeDtos.ToList());

                return routeDtos;
            }
            else
            {
                return cachedValue;
            }
        }

        public async Task<RouteDto> GetByIdAsync(string id)
        {
            var cacheKey = _cacheConfigruationMangementService.GetKeyById<RouteDto>(new RouteDto { Id = id });
            var cachedValue = _cacheService.GetRecordFromCacheAsync<RouteDto>(cacheKey).Result;

            if (cachedValue is null || cachedValue is default(RouteDto))
            {
                var result = await DatabaseAdapter.GetAsync<Route>(id: id);

                var routeDto = _mapper.Map<RouteDto>(result);

                await _cacheService.SetRecordInCacheAsync<RouteDto>(cacheKey, routeDto);

                return routeDto;
            }
            else
            {
                return cachedValue;
            }
        }

        public async Task<Tuple<RouteDto, RouteDto>> PatchAsync(string id, JsonPatchDocument<PatchRoute> jsonPatchDocument)
        {
            var athleteEntity = await DatabaseAdapter.GetAsync<Route>(id: id);

            if (athleteEntity is null)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var originaltRoleDto = _mapper.Map<RouteDto>(athleteEntity);

            jsonPatchDocument.Map<PatchRoute, Route>().ApplyTo(athleteEntity);
            athleteEntity.UpdatedBy = "updated-by-user";
            athleteEntity.UpdatedDate = DateTime.Now;

            Expression<Func<Route, bool>> expression = x => x.Id == athleteEntity.Id;

            var result = await DatabaseAdapter.UpsertAsync<Route>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);

            return new Tuple<RouteDto, RouteDto>(originaltRoleDto, _mapper.Map<RouteDto>(result));
        }

        public async Task<RouteDto> SaveAsync(RouteDto routeDto)
        {
            var cacheKey1 = _cacheConfigruationMangementService.GetCustomKey<RouteDto>(routeDto);
            var cachedValue = _cacheService.GetRecordFromCacheAsync<RouteDto>(cacheKey1).Result;
            var overrideCache = false;

            if (cachedValue is null || cachedValue is default(RouteDto))
            {
                var exception = new Exception("Invalid Cache");
                TelemetryClient.TrackException(exception);
                overrideCache = true;
            }

            if (cachedValue is null
                || cachedValue is default(RouteDto)
                || overrideCache)
            {

                routeDto.Id = string.IsNullOrEmpty(routeDto.Id) ? DatabaseAdapter.GenerateNewId() : routeDto.Id;
//                routeDto.RouteId = string.IsNullOrEmpty(routeDto.RouteId) ? routeDto.Id : routeDto.RouteId;

                var cacheKey2 = _cacheConfigruationMangementService.GetKeyById<RouteDto>(routeDto);

                var athleteEntity = _mapper.Map<Route>(routeDto);
                athleteEntity.SetAuditDefaults();

                Expression<Func<Route, bool>> expression = x => x.Id == routeDto.Id;
                var result = default(Route);

                try
                {
                    result = await DatabaseAdapter.UpsertAsync<Route>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);
                }
                catch (MongoCommandException)
                {

                    // Do to MongoDb bug in Upsert
                    // which sometimes throws:
                    /*
					 * Command findAndModify failed: Plan executor error during 
					 * findAndModify :: caused by :: After applying the update, 
					 * the (immutable) field '_id' was found to have been altered to _id:
					 *
					 */
                    var existingRoleEntity = await DatabaseAdapter.GetAsync(expression, new CancellationToken());

                    athleteEntity.Id = existingRoleEntity.Id;
                    result = await DatabaseAdapter.UpsertAsync<Route>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);
                }

                var savedRoleDto = _mapper.Map<RouteDto>(result);

                await _cacheService.SetRecordInCacheAsync<RouteDto>(cacheKey1, savedRoleDto);
                await _cacheService.SetRecordInCacheAsync<RouteDto>(cacheKey2, savedRoleDto);
                await _cacheService.AddEntityToListCacheAsync(nameof(Route), savedRoleDto);

                return savedRoleDto;
            }
            else
            {
                return JsonConvert.DeserializeObject<RouteDto>(JsonConvert.SerializeObject(cachedValue));
            }
        }
    }

}
