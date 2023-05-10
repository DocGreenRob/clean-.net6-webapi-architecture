using Microsoft.AspNetCore.JsonPatch;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Common.Models.Patch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CGE.CleanCode.Service.Interfaces
{
    public interface IRouteService
    {
        Task DeleteAsync(string id);
        Task<IEnumerable<RouteDto>> GetAllAsync();
        Task<RouteDto> GetByIdAsync(string id);
        Task<RouteDto> SaveAsync(RouteDto RouteDto);
        Task<Tuple<RouteDto, RouteDto>> PatchAsync(string id, JsonPatchDocument<PatchRoute> jsonPatchDocument);
    }
}
