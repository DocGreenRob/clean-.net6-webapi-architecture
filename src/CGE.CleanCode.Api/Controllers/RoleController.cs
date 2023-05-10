using AutoMapper;
using Cge.Core.Extensions;
using Cge.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CGE.CleanCode.Api.Models.Authorization;
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

    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMapper _mapper;

        public RoleController(ITelemetryClient telemetryClient,
                                    IConfiguration configuration,
                                    IRoleService roleService,
                                    IMessagePublisher messagePublisher,
                                    IMapper mapper) : base(telemetryClient, configuration)
        {
            _roleService = roleService.ValidateArgNotNull(nameof(roleService));
            _messagePublisher = messagePublisher.ValidateArgNotNull(nameof(messagePublisher));
            _mapper = mapper.ValidateArgNotNull(nameof(mapper));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _roleService.DeleteAsync(id);

            var recordDeletedMessage = new RecordDeletedMessage
            {
                EntityType = "Role",
                Id = id
            };

            _messagePublisher.Initialize(Configuration["ServiceBus:Queues:RecordDelete"]);

            await _messagePublisher.PublishMessage(recordDeletedMessage);

            TelemetryClient.TrackEvent($"RecordDelete", new Dictionary<string, string>
            {
                { "Role", id }
            });

            return Ok();
        }

        [HttpGet()]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllAsync()
        {
            var results = await _roleService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAsync(string id)
        {
            var result = await _roleService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost()]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ApiAuthorize(nameof(HttpMethods.Get), "api/v1/Role")]
        public async Task<IActionResult> SaveAsync(RoleDto roleDto)
        {
            var result = await _roleService.SaveAsync(roleDto);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(Task<IActionResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchAsync(string id, JsonPatchDocument<PatchRole> jsonPatchDocument)
        {
            var roleDto = await _roleService.GetByIdAsync(id).ConfigureAwait(false);
            var roleEntity = _mapper.Map<Role>(roleDto);
            jsonPatchDocument.Map<PatchRole, Role>().ApplyTo(roleEntity);
            roleDto = _mapper.Map<RoleDto>(roleEntity);

            // TODO: Ensure other controllers' PatchAsync validates the Dto prior to sendind to Service
            var validationResult = new RoleValidator().Validate(roleDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var result = await _roleService.PatchAsync(id, jsonPatchDocument);

            var recordPatchedMessage = new RecordPatchedMessage<RoleDto>
            {
                OldRecord = result.Item1,
                NewRecord = result.Item2
            };

            _messagePublisher.Initialize(Configuration["ServiceBus:Queues:RecordPatch"]);

            var userProperties = new Dictionary<string, string>
            {
                { "Role", id }
            };

            await _messagePublisher.PublishMessage(recordPatchedMessage, userProperties);

            TelemetryClient.TrackEvent($"RecordPatch", userProperties);

            return Ok();
        }
    }
}
