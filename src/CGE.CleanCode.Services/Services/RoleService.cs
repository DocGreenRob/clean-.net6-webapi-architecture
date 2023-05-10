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
    public class RoleService : BaseService<Role>, IRoleService
    {
        private readonly string _cacheReplaceSpaceString;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ICacheConfigruationMangementService _cacheConfigruationMangementService;

        public RoleService(IMapper mapper,
                                IConfiguration configuration,
                                ICacheService cacheService,
                                ITelemetryClient telemetryClient,
                                IMongoDbWrapper mongoDbWrapper,
                                ICacheConfigruationMangementService cacheConfigruationMangementService) : base(mongoDbWrapper,
                                                                                                            configuration,
                                                                                                            telemetryClient,
                                                                                                            Globals.Documents.Role)
        {
            // Validate stuff
            _cacheService = cacheService.ValidateArgNotNull(nameof(cacheService));
            _mapper = mapper.ValidateArgNotNull(nameof(mapper));
            _cacheConfigruationMangementService = cacheConfigruationMangementService.ValidateArgNotNull(nameof(cacheConfigruationMangementService));
        }

        public async Task DeleteAsync(string id)
        {
            var result = await DatabaseAdapter.GetAsync<Role>(id: id);

            if (result == null || result == default(Role))
            {
                throw new Exception();
            }

            result.IsDeleted = true;
            result.UpdatedDate = DateTime.Now;
            result.UpdatedBy = "updated-by-user";

            await DatabaseAdapter.UpsertAsync(id, result);
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var cacheKey = _cacheConfigruationMangementService.GetListKey<RoleDto>();
            var cachedValue = _cacheService.GetRecordFromCacheAsync<List<RoleDto>>(cacheKey).Result;

            if (cachedValue is null || cachedValue is default(List<RoleDto>))
            {
                var results = await DatabaseAdapter.GetAllAsync<Role>(x => !x.IsDeleted);
                var roleDtos = results.Select(x => _mapper.Map<RoleDto>(x)).AsEnumerable();

                await _cacheService.SetRecordInCacheAsync<List<RoleDto>>(cacheKey, roleDtos.ToList());

                return roleDtos;
            }
            else
            {
                return cachedValue;
            }
        }

        public async Task<RoleDto> GetByIdAsync(string id)
        {
            var cacheKey = _cacheConfigruationMangementService.GetKeyById<RoleDto>(new RoleDto { Id = id });
            var cachedValue = _cacheService.GetRecordFromCacheAsync<RoleDto>(cacheKey).Result;

            if (cachedValue is null || cachedValue is default(RoleDto))
            {
                var result = await DatabaseAdapter.GetAsync<Role>(id: id);

                var roleDto = _mapper.Map<RoleDto>(result);

                await _cacheService.SetRecordInCacheAsync<RoleDto>(cacheKey, roleDto);

                return roleDto;
            }
            else
            {
                return cachedValue;
            }
        }

        public async Task<Tuple<RoleDto, RoleDto>> PatchAsync(string id, JsonPatchDocument<PatchRole> jsonPatchDocument)
        {
            var athleteEntity = await DatabaseAdapter.GetAsync<Role>(id: id);

            if (athleteEntity is null)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var originaltRoleDto = _mapper.Map<RoleDto>(athleteEntity);

            jsonPatchDocument.Map<PatchRole, Role>().ApplyTo(athleteEntity);
            athleteEntity.UpdatedBy = "updated-by-user";
            athleteEntity.UpdatedDate = DateTime.Now;

            Expression<Func<Role, bool>> expression = x => x.Id == athleteEntity.Id;

            var result = await DatabaseAdapter.UpsertAsync<Role>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);

            return new Tuple<RoleDto, RoleDto>(originaltRoleDto, _mapper.Map<RoleDto>(result));
        }

        public async Task<RoleDto> SaveAsync(RoleDto roleDto)
        {
            var cacheKey1 = _cacheConfigruationMangementService.GetCustomKey<RoleDto>(roleDto);
            var cachedValue = _cacheService.GetRecordFromCacheAsync<RoleDto>(cacheKey1).Result;
            var overrideCache = false;

            if (cachedValue is null || cachedValue is default(RoleDto))
            {
                var exception = new Exception("Invalid Cache");
                TelemetryClient.TrackException(exception);
                overrideCache = true;
            }

            if (cachedValue is null
                || cachedValue is default(RoleDto)
                || overrideCache)
            {

                roleDto.Id = string.IsNullOrEmpty(roleDto.Id) ? DatabaseAdapter.GenerateNewId() : roleDto.Id;
                //roleDto.RoleId = string.IsNullOrEmpty(roleDto.RoleId) ? roleDto.Id : roleDto.RoleId;
                var cacheKey2 = _cacheConfigruationMangementService.GetKeyById<RoleDto>(roleDto);

                var athleteEntity = _mapper.Map<Role>(roleDto);
                athleteEntity.SetAuditDefaults();

                Expression<Func<Role, bool>> expression = x => x.Id == roleDto.Id;
                var result = default(Role);

                try
                {
                    result = await DatabaseAdapter.UpsertAsync<Role>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);
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
                    result = await DatabaseAdapter.UpsertAsync<Role>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);
                }

                var savedRoleDto = _mapper.Map<RoleDto>(result);

                await _cacheService.SetRecordInCacheAsync<RoleDto>(cacheKey1, savedRoleDto);
                await _cacheService.SetRecordInCacheAsync<RoleDto>(cacheKey2, savedRoleDto);
                await _cacheService.AddEntityToListCacheAsync(nameof(Role), savedRoleDto);

                return savedRoleDto;
            }
            else
            {
                return JsonConvert.DeserializeObject<RoleDto>(JsonConvert.SerializeObject(cachedValue));
            }
        }
    }
}
