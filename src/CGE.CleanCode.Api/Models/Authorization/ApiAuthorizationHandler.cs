using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace CGE.CleanCode.Api.Models.Authorization
{
    public class ApiAuthorizationHandler : AuthorizationHandler<ApiRolePolicyRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiRolePolicyRequirement requirement)
        {
            if (context.User.Claims.Any(x => x.Value == requirement.Role))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
