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
    public class UserService : BaseService<User>, IUserService
    {
        private readonly string _cacheReplaceSpaceString;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ICacheConfigruationMangementService _cacheConfigruationMangementService;

        public UserService(IMapper mapper,
                                IConfiguration configuration,
                                ICacheService cacheService,
                                ITelemetryClient telemetryClient,
                                IMongoDbWrapper mongoDbWrapper,
                                ICacheConfigruationMangementService cacheConfigruationMangementService) : base(mongoDbWrapper,
                                                                                                            configuration,
                                                                                                            telemetryClient,
                                                                                                            Globals.Documents.User)
        {
            // Validate stuff
            _cacheService = cacheService.ValidateArgNotNull(nameof(cacheService));
            _mapper = mapper.ValidateArgNotNull(nameof(mapper));
            _cacheConfigruationMangementService = cacheConfigruationMangementService.ValidateArgNotNull(nameof(cacheConfigruationMangementService));
        }

        public async Task DeleteAsync(string id)
        {
            var result = await DatabaseAdapter.GetAsync<User>(id: id);

            if (result == null || result == default(User))
            {
                throw new Exception();
            }

            result.IsDeleted = true;
            result.UpdatedDate = DateTime.Now;
            result.UpdatedBy = "updated-by-user";

            await DatabaseAdapter.UpsertAsync(id, result);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var cacheKey = _cacheConfigruationMangementService.GetListKey<UserDto>();
            var cachedValue = _cacheService.GetRecordFromCacheAsync<List<UserDto>>(cacheKey).Result;

            if (cachedValue is null || cachedValue is default(List<UserDto>))
            {
                var results = await DatabaseAdapter.GetAllAsync<User>(x => !x.IsDeleted);
                var userDtos = results.Select(x => _mapper.Map<UserDto>(x)).AsEnumerable();

                await _cacheService.SetRecordInCacheAsync<List<UserDto>>(cacheKey, userDtos.ToList());

                return userDtos;
            }
            else
            {
                return cachedValue;
            }
        }

        public async Task<UserDto> GetByIdAsync(string id)
        {
            var cacheKey = _cacheConfigruationMangementService.GetKeyById<UserDto>(new UserDto { Id = id });
            var cachedValue = _cacheService.GetRecordFromCacheAsync<UserDto>(cacheKey).Result;

            if (cachedValue is null || cachedValue is default(UserDto))
            {
                var result = await DatabaseAdapter.GetAsync<User>(id: id);

                var userDto = _mapper.Map<UserDto>(result);

                await _cacheService.SetRecordInCacheAsync<UserDto>(cacheKey, userDto);

                return userDto;
            }
            else
            {
                return cachedValue;
            }
        }

        public async Task<Tuple<UserDto, UserDto>> PatchAsync(string id, JsonPatchDocument<PatchUser> jsonPatchDocument)
        {
            var athleteEntity = await DatabaseAdapter.GetAsync<User>(id: id);

            if (athleteEntity is null)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var originaltRoleDto = _mapper.Map<UserDto>(athleteEntity);

            jsonPatchDocument.Map<PatchUser, User>().ApplyTo(athleteEntity);
            athleteEntity.UpdatedBy = "updated-by-user";
            athleteEntity.UpdatedDate = DateTime.Now;

            Expression<Func<User, bool>> expression = x => x.Id == athleteEntity.Id;

            var result = await DatabaseAdapter.UpsertAsync<User>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);

            return new Tuple<UserDto, UserDto>(originaltRoleDto, _mapper.Map<UserDto>(result));
        }

        public async Task<UserDto> SaveAsync(UserDto userDto)
        {
            var cacheKey1 = _cacheConfigruationMangementService.GetCustomKey<UserDto>(userDto);
            var cachedValue = _cacheService.GetRecordFromCacheAsync<UserDto>(cacheKey1).Result;
            var overrideCache = false;

            if (cachedValue is null || cachedValue is default(UserDto))
            {
                var exception = new Exception("Invalid Cache");
                TelemetryClient.TrackException(exception);
                overrideCache = true;
            }

            if (cachedValue is null
                || cachedValue is default(UserDto)
                || overrideCache)
            {

                userDto.Id = string.IsNullOrEmpty(userDto.Id) ? DatabaseAdapter.GenerateNewId() : userDto.Id;
                //userDto.UserId = string.IsNullOrEmpty(userDto.UserId) ? userDto.Id : userDto.UserId;

                var cacheKey2 = _cacheConfigruationMangementService.GetKeyById<UserDto>(userDto);

                var athleteEntity = _mapper.Map<User>(userDto);
                athleteEntity.SetAuditDefaults();

                Expression<Func<User, bool>> expression = x => x.Id  == userDto.Id;
                var result = default(User);

                try
                {
                    result = await DatabaseAdapter.UpsertAsync<User>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);
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
                    result = await DatabaseAdapter.UpsertAsync<User>(expression, athleteEntity, new CancellationToken()).ConfigureAwait(false);
                }

                var savedRoleDto = _mapper.Map<UserDto>(result);

                await _cacheService.SetRecordInCacheAsync<UserDto>(cacheKey1, savedRoleDto);
                await _cacheService.SetRecordInCacheAsync<UserDto>(cacheKey2, savedRoleDto);
                await _cacheService.AddEntityToListCacheAsync(nameof(User), savedRoleDto);

                return savedRoleDto;
            }
            else
            {
                return JsonConvert.DeserializeObject<UserDto>(JsonConvert.SerializeObject(cachedValue));
            }
        }
    }

}
