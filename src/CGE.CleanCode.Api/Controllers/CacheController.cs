using Cge.Core.Extensions;
using Cge.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CGE.CleanCode.Api.Models.Cache;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Service.Interfaces;
using System.Threading.Tasks;

namespace CGE.CleanCode.Api.Controllers
{
	[ApiVersion("1.0")]
	//[Authorize]
	[ApiController]
	[Route("api/v{version:apiVersion}/[controller]")]

	public class CacheController : BaseController
	{
		private readonly ICacheService _cacheService;

		public CacheController(ITelemetryClient telemetryClient,
								IConfiguration configuration,
								ICacheService cacheService) : base(telemetryClient, configuration)
		{
			_cacheService = cacheService.ValidateArgNotNull(nameof(cacheService));
		}

		[HttpPost("add-to-list/{cacheKey}")]
		[ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
		//[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AddEntityFromListCacheAsync(string cacheKey, NewRecordDto newRecordDto)
		{
			var listKey = string.Empty;

			var serializedRecord = JsonConvert.SerializeObject(newRecordDto.Value);

			if (newRecordDto.Type == nameof(RoleDto))
			{
				var deserializedRoleDto = JsonConvert.DeserializeObject<RoleDto>(serializedRecord);

				// TODO: Make Enum
				await _cacheService.AddEntityToListCacheAsync<RoleDto>("Role", deserializedRoleDto);
			}

			if (newRecordDto.Type == nameof(RolePermissionDto))
			{
				var deserializedRolePermissionDto = JsonConvert.DeserializeObject<RolePermissionDto>(serializedRecord);

				// TODO: Make Enum
				await _cacheService.AddEntityToListCacheAsync<RolePermissionDto>("RolePermission", deserializedRolePermissionDto);
			}

			if (newRecordDto.Type == nameof(RouteDto))
			{
				var deserializedRouteDto  = JsonConvert.DeserializeObject<RouteDto>(serializedRecord);

				// TODO: Make Enum
				await _cacheService.AddEntityToListCacheAsync<RouteDto>("Route", deserializedRouteDto);
			}

			if (newRecordDto.Type == nameof(UserDto))
			{
				var deserializedUserDto = JsonConvert.DeserializeObject<UserDto>(serializedRecord);

				// TODO: Make Enum
				await _cacheService.AddEntityToListCacheAsync<UserDto>("User", deserializedUserDto);
			}

			

			return Ok();
		}

		[HttpPost("add-key")]
		[ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
		//[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AddKey(NewRecordDto newRecordDto)
		{
			await _cacheService.SetRecordInCacheAsync<object>(newRecordDto.Key, newRecordDto.Value);
			return Ok();
		}

		[HttpDelete("all")]
		[ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
		//[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> RemoveAllKeys(string cacheKey)
		{
			_cacheService.RemoveAllKeys();
			return Ok();
		}

		[HttpDelete("{entityType}/{id}")]
		[ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
		//[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> RemoveEntityFromListCacheAsync(string entityType, string id)
		{
			await _cacheService.RemoveEntityFromListNameAndIdCacheAsync(entityType, id);
			return Ok();
		}

		[HttpDelete("{cacheKey}")]
		[ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
		//[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> RemoveKey(string cacheKey)
		{
			_cacheService.RemoveKey(cacheKey);
			return Ok();
		}

		[HttpGet("all")]
		[ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
		//[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetAllKeys()
		{
			var result = await _cacheService.GetAllKeys();
			return Ok(result);
		}
	}
}
