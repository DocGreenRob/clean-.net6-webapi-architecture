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
    public class RolePermissionService : BaseService<RolePermission>, IRolePermissionService
    {
        private readonly string _cacheReplaceSpaceString;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ICacheConfigruationMangementService _cacheConfigruationMangementService;

        public RolePermissionService(IMapper mapper,
                                IConfiguration configuration,
                                ICacheService cacheService,
                                ITelemetryClient telemetryClient,
                                IMongoDbWrapper mongoDbWrapper,
                                ICacheConfigruationMangementService cacheConfigruationMangementService) : base(mongoDbWrapper,
                                                                                                            configuration,
                                                                                                            telemetryClient,
                                                                                                            Globals.Documents.RolePermission)
        {
            // Validate stuff
            _cacheService = cacheService.ValidateArgNotNull(nameof(cacheService));
            _mapper = mapper.ValidateArgNotNull(nameof(mapper));
            _cacheConfigruationMangementService = cacheConfigruationMangementService.ValidateArgNotNull(nameof(cacheConfigruationMangementService));
        }

        public async Task DeleteAsync(string id)
        {
            var result = await DatabaseAdapter.GetAsync<RolePermission>(id: id);

            if (result == null || result == default(RolePermission))
            {
                throw new Exception();
            }

            result.IsDeleted = true;
            result.UpdatedDate = DateTime.Now;
            result.UpdatedBy = "updated-by-user";

            await DatabaseAdapter.UpsertAsync(id, result);
        }

        public async Task<IEnumerable<RolePermissionDto>> GetAllAsync()
        {
            var cacheKey = _cacheConfigruationMangementService.GetListKey<RolePermissionDto>();
            var cachedValue = _cacheService.GetRecordFromCacheAsync<List<RolePermissionDto>>(cacheKey).Result;

            if (cachedValue is null || cachedValue is default(List<RolePermissionDto>))
            {
                var results = await DatabaseAdapter.GetAllAsync<RolePermission>(x => !x.IsDeleted);
                var roleDtos = results.Select(x => _mapper.Map<RolePermissionDto>(x)).AsEnumerable();

                await _cacheService.SetRecordInCacheAsync<List<RolePermissionDto>>(cacheKey, roleDtos.ToList());

                return roleDtos;
            }
            else
            {
                return cachedValue;
            }
        }

        public async Task<RolePermissionDto> GetByIdAsync(string id)
        {
            var cacheKey = _cacheConfigruationMangementService.GetKeyById<RolePermissionDto>(new RolePermissionDto { Id = id });
            var cachedValue = _cacheService.GetRecordFromCacheAsync<RolePermissionDto>(cacheKey).Result;

            if (cachedValue is null || cachedValue is default(RolePermissionDto))
            {
                var result = await DatabaseAdapter.GetAsync<RolePermission>(id: id);

                var dto = _mapper.Map<RolePermissionDto>(result);

                await _cacheService.SetRecordInCacheAsync<RolePermissionDto>(cacheKey, dto);

                return dto;
            }
            else
            {
                return cachedValue;
            }
        }

        public async Task<Tuple<RolePermissionDto, RolePermissionDto>> PatchAsync(string id, JsonPatchDocument<PatchRolePermission> jsonPatchDocument)
        {
            var rolePermissionEntity = await DatabaseAdapter.GetAsync<RolePermission>(id: id);

            if (rolePermissionEntity is null)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var originaltRoleDto = _mapper.Map<RolePermissionDto>(rolePermissionEntity);

            jsonPatchDocument.Map<PatchRolePermission, RolePermission>().ApplyTo(rolePermissionEntity);
            rolePermissionEntity.UpdatedBy = "updated-by-user";
            rolePermissionEntity.UpdatedDate = DateTime.Now;

            Expression<Func<RolePermission, bool>> expression = x => x.Id == rolePermissionEntity.Id;

            var result = await DatabaseAdapter.UpsertAsync<RolePermission>(expression, rolePermissionEntity, new CancellationToken()).ConfigureAwait(false);

            return new Tuple<RolePermissionDto, RolePermissionDto>(originaltRoleDto, _mapper.Map<RolePermissionDto>(result));
        }

        public async Task<RolePermissionDto> SaveAsync(RolePermissionDto rolePermissionDto)
        {
            var cacheKey1 = _cacheConfigruationMangementService.GetCustomKey<RolePermissionDto>(rolePermissionDto);
            var cachedValue = _cacheService.GetRecordFromCacheAsync<RolePermissionDto>(cacheKey1).Result;
            var overrideCache = false;

            if (cachedValue is null || cachedValue is default(RolePermissionDto))
            {
                var exception = new Exception("Invalid Cache");
                TelemetryClient.TrackException(exception);
                overrideCache = true;
            }

            if (cachedValue is null
                || cachedValue is default(RolePermissionDto)
                || overrideCache)
            {

                rolePermissionDto.Id = string.IsNullOrEmpty(rolePermissionDto.Id) ? DatabaseAdapter.GenerateNewId() : rolePermissionDto.Id;
                rolePermissionDto.RoleId = string.IsNullOrEmpty(rolePermissionDto.RoleId) ? rolePermissionDto.RoleId : rolePermissionDto.RoleId;
                var cacheKey2 = _cacheConfigruationMangementService.GetKeyById<RolePermissionDto>(rolePermissionDto);

                var athleteEntity = _mapper.Map<RolePermission>(rolePermissionDto);
                athleteEntity.SetAuditDefaults();

                Expression<Func<RolePermission, bool>> expression = x => x.RoleId == rolePermissionDto.RoleId;
                var result = default(RolePermission);

                try
                {
                    result = await DatabaseAdapter.UpsertAsync<RolePermission>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);
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
                    result = await DatabaseAdapter.UpsertAsync<RolePermission>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);
                }

                var savedRoleDto = _mapper.Map<RolePermissionDto>(result);

                await _cacheService.SetRecordInCacheAsync<RolePermissionDto>(cacheKey1, savedRoleDto);
                await _cacheService.SetRecordInCacheAsync<RolePermissionDto>(cacheKey2, savedRoleDto);
                await _cacheService.AddEntityToListCacheAsync(nameof(RolePermission), savedRoleDto);

                return savedRoleDto;
            }
            else
            {
                return JsonConvert.DeserializeObject<RolePermissionDto>(JsonConvert.SerializeObject(cachedValue));
            }
        }
    }
}
