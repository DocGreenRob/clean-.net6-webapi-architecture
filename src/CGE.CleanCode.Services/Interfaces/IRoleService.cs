using Microsoft.AspNetCore.JsonPatch;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Common.Models.Patch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CGE.CleanCode.Service.Interfaces
{
    public interface IRoleService
    {
        Task DeleteAsync(string id);
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto> GetByIdAsync(string id);
        Task<RoleDto> SaveAsync(RoleDto RoleDto);
        Task<Tuple<RoleDto, RoleDto>> PatchAsync(string id, JsonPatchDocument<PatchRole> jsonPatchDocument);
    }
}
