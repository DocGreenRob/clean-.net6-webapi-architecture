using System.Collections.Generic;
using System.Net.Http;
using CGE.CleanCode.Service.Interfaces;
using CGE.CleanCode.Common.Models.Dto;
using System.Linq;

namespace CGE.CleanCode.Api.Models.Authorization
{
    public static class AuthorizationApiSet
    {
        public static Dictionary<ApiDetail, IEnumerable<string>> ApiRoles;
        private static IRouteService _routeService;
        static AuthorizationApiSet()
        {
            List<RouteDto> lstRoutes =  _routeService.GetAllAsync().Result.ToList();


            ApiRoles = new Dictionary<ApiDetail, IEnumerable<string>>();
            // Transactions
            ApiRoles.Add(new ApiDetail("api/v1/transactions", new string[] { nameof(HttpMethod.Get), nameof(HttpMethod.Post) }), new string[] { "Cashier", "Admin" });
            ApiRoles.Add(new ApiDetail("api/v1/transactions/transactionsummary", new string[] { nameof(HttpMethod.Get) }), new string[] { "Cashier", "Admin" });
            ApiRoles.Add(new ApiDetail("api/v1/transactions/void", new string[] { nameof(HttpMethod.Put) }), new string[] { "Cashier", "Admin" });
            ApiRoles.Add(new ApiDetail("api/v1/transactions/reversal", new string[] { nameof(HttpMethod.Post) }), new string[] { "Cashier", "Admin" });
            ApiRoles.Add(new ApiDetail("api/v1/transactions/adjust", new string[] { nameof(HttpMethod.Post) }), new string[] { "Cashier", "Admin" });
            ApiRoles.Add(new ApiDetail("api/v1/transactions/refund", new string[] { nameof(HttpMethod.Post) }), new string[] { "Cashier", "Admin" });
            ApiRoles.Add(new ApiDetail("api/v1/transactions/generate", new string[] { nameof(HttpMethod.Post) }), new string[] { "Cashier", "Admin" });
            ApiRoles.Add(new ApiDetail("api/v1/transactions/devices", new string[] { nameof(HttpMethod.Get) }), new string[] { "Cashier", "Admin" });
            ApiRoles.Add(new ApiDetail("api/v1/transactions/searchsummary", new string[] { nameof(HttpMethod.Get) }), new string[] { "Cashier", "Admin" });
            ApiRoles.Add(new ApiDetail("api/v1/transactions/email", new string[] { nameof(HttpMethod.Post) }), new string[] { "Cashier", "Admin" });
            // GL Payments
            ApiRoles.Add(new ApiDetail("api/v1/transactions/glpayments", new string[] {
                nameof(HttpMethod.Get),
                nameof(HttpMethod.Post),
                nameof(HttpMethod.Put)
            }), new string[] { "Cashier", "Admin" });
            // Merchant Configuration
            ApiRoles.Add(new ApiDetail("api/v1/merchant/configuration", new string[] {
                nameof(HttpMethod.Get),
                nameof(HttpMethod.Post)
            }), new string[] { "Cashier", "Admin" });
            // Metadata
            ApiRoles.Add(new ApiDetail("api/v1/transactions/searchsummary", new string[] { nameof(HttpMethod.Post) }), new string[] { "Admin" });
        }

       
    }
}
