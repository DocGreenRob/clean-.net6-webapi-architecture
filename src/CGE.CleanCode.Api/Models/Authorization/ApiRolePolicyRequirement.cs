using Microsoft.AspNetCore.Authorization;

namespace CGE.CleanCode.Api.Models.Authorization
{
    public class ApiRolePolicyRequirement : IAuthorizationRequirement
    {
        public string Role { get; set; }
        public ApiRolePolicyRequirement(string role) { Role = role; }
    }
}
