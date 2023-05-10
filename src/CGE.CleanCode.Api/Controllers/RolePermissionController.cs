using AutoMapper;
using Cge.Core.Extensions;
using Cge.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CGE.CleanCode.Api.Models.ServiceBus;
using CGE.CleanCode.Api.Models.Validators;
using CGE.CleanCode.Common.Extensions;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Common.Models.Patch;
using CGE.CleanCode.Dal.Entities;
using CGE.CleanCode.Service.Interfaces;
using CGE.CleanCode.ServiceBus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CGE.CleanCode.Api.Controllers
{
    [ApiVersion("1.0")]
    //[Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class RolePermissionController : BaseController
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMapper _mapper;

        public RolePermissionController(ITelemetryClient telemetryClient,
                                    IConfiguration configuration,
                                    IRolePermissionService rolePermissionService,
                                    IMessagePublisher messagePublisher,
                                    IMapper mapper) : base(telemetryClient, configuration)
        {
            _rolePermissionService = rolePermissionService.ValidateArgNotNull(nameof(rolePermissionService));
            _messagePublisher = messagePublisher.ValidateArgNotNull(nameof(messagePublisher));
            _mapper = mapper.ValidateArgNotNull(nameof(mapper));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _rolePermissionService.DeleteAsync(id);

            var recordDeletedMessage = new RecordDeletedMessage
            {
                EntityType = "RolePermission",
                Id = id
            };

            _messagePublisher.Initialize(Configuration["ServiceBus:Queues:RecordDelete"]);

            await _messagePublisher.PublishMessage(recordDeletedMessage);

            TelemetryClient.TrackEvent($"RecordDelete", new Dictionary<string, string>
            {
                { "RolePermission", id }
            });

            return Ok();
        }

        [HttpGet()]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllAsync()
        {
            var results = await _rolePermissionService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAsync(string id)
        {
            var result = await _rolePermissionService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost()]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SaveAsync(RolePermissionDto rolePermissionDto)
        {
            var result = await _rolePermissionService.SaveAsync(rolePermissionDto);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchAsync(string id, JsonPatchDocument<PatchRolePermission> jsonPatchDocument)
        {
            var rolePermissionDto = await _rolePermissionService.GetByIdAsync(id).ConfigureAwait(false);
            var rolePermissionEntity = _mapper.Map<RolePermission>(rolePermissionDto);
            jsonPatchDocument.Map<PatchRolePermission, RolePermission>().ApplyTo(rolePermissionEntity);
            rolePermissionDto = _mapper.Map<RolePermissionDto>(rolePermissionEntity);

            // TODO: Ensure other controllers' PatchAsync validates the Dto prior to sendind to Service
            var validationResult = new RolePermissionValidator().Validate(rolePermissionDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var result = await _rolePermissionService.PatchAsync(id, jsonPatchDocument);

            var recordPatchedMessage = new RecordPatchedMessage<RolePermissionDto>
            {
                OldRecord = result.Item1,
                NewRecord = result.Item2
            };

            _messagePublisher.Initialize(Configuration["ServiceBus:Queues:RecordPatch"]);

            var userProperties = new Dictionary<string, string>
            {
                { "RolePermission", id }
            };

            await _messagePublisher.PublishMessage(recordPatchedMessage, userProperties);

            TelemetryClient.TrackEvent($"RecordPatch", userProperties);

            return Ok();
        }
    }
}
