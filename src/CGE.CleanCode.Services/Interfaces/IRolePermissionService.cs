using Microsoft.AspNetCore.JsonPatch;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Common.Models.Patch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CGE.CleanCode.Service.Interfaces
{
    public interface IRolePermissionService
    {
        Task DeleteAsync(string id);
        Task<IEnumerable<RolePermissionDto>> GetAllAsync();
        Task<RolePermissionDto> GetByIdAsync(string id);
        Task<RolePermissionDto> SaveAsync(RolePermissionDto RolePermissionDto);
        Task<Tuple<RolePermissionDto, RolePermissionDto>> PatchAsync(string id, JsonPatchDocument<PatchRolePermission> jsonPatchDocument);
    }
}
