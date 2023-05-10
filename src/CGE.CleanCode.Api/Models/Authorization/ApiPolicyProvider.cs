using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using CGE.CleanCode.Common.Models.Dto;
using CGE.CleanCode.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using static CGE.CleanCode.Api.Models.Authorization.AuthorizationApiSet;

namespace CGE.CleanCode.Api.Models.Authorization
{
    public class ApiPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
        private readonly IRouteService _routeService;
        private readonly IRoleService _roleService;
        private readonly IRolePermissionService _rolePermissionService;
        public ApiPolicyProvider(IOptions<AuthorizationOptions> options,
            IRouteService routeService, 
            IRoleService roleService,
            IRolePermissionService rolePermissionService)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            _routeService = routeService;
            _roleService = roleService;
            _rolePermissionService = rolePermissionService;
        }


        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string httpVerb, string url)
        {
            url = url.ToLower();
            Dictionary<ApiDetail, IEnumerable<string>> ApiRoles = new Dictionary<ApiDetail, IEnumerable<string>>();

            List<RouteDto> lstRoutes = _routeService.GetAllAsync().Result.ToList();
            foreach (RouteDto route in lstRoutes)
            {
                List<RolePermissionDto> rolePermissions = _rolePermissionService.GetAllAsync().Result.ToList().Where(x => x.RouteId == route.Id).ToList();
                if (rolePermissions.Any())
                {
                    foreach (RolePermissionDto rolePermission in rolePermissions)
                    {
                        List<string> roles = new List<string>();
                        List<string> verbs = new List<string>();

                        RoleDto roleDto = _roleService.GetByIdAsync(rolePermission.RoleId).Result;
                        if (roleDto != null)
                            roles.Add(roleDto.RoleName);

                        verbs.AddRange(rolePermission.HttpVerb);

                        ApiRoles.Add(new ApiDetail(route.Url.ToLower(), verbs.ToArray()), roles.ToArray());
                    }

                }
            }
            

            if (ApiRoles.Any(x => x.Key.Url == url && x.Key.HttpVerbs.Contains(httpVerb)))
            {
                var policy = new AuthorizationPolicyBuilder();
                foreach (var role in ApiRoles.Where(x => x.Key.Url == url && x.Key.HttpVerbs.Contains(httpVerb)))
                {
                    policy.AddRequirements(new ApiRolePolicyRequirement(((string[])role.Value)[0]));
                }
                return Task.FromResult(policy.Build());
            }

            //if (AuthorizationApiSet.ApiRoles.Any(x => x.Key.Url == url && x.Key.HttpVerbs.Contains(httpVerb)))
            //{
            //    var policy = new AuthorizationPolicyBuilder();
            //    foreach (var role in AuthorizationApiSet.ApiRoles.FirstOrDefault(x => x.Key.Url == url && x.Key.HttpVerbs.Contains(httpVerb)).Value)
            //    {
            //        policy.AddRequirements(new ApiRolePolicyRequirement(role));
            //    }
            //    return Task.FromResult(policy.Build());
            //}

            return FallbackPolicyProvider.GetPolicyAsync("");
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var a = policyName.Split(":");
            return GetPolicyAsync(a[0], a[1]);
        }
    }
}
