using Microsoft.AspNetCore.JsonPatch;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Common.Models.Patch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CGE.CleanCode.Service.Interfaces
{
    public interface IUserService
    {
        Task DeleteAsync(string id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(string id);
        Task<UserDto> SaveAsync(UserDto UserDto);
        Task<Tuple<UserDto, UserDto>> PatchAsync(string id, JsonPatchDocument<PatchUser> jsonPatchDocument);
    }
}
