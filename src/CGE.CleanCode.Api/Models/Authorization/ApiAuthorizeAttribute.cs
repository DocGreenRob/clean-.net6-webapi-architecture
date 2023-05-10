using Microsoft.AspNetCore.Authorization;

namespace CGE.CleanCode.Api.Models.Authorization
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public ApiAuthorizeAttribute(string verb, string url = "") => Url = $"{verb}:{url}";

        public string Url
        {
            get
            {
                return Policy;
            }
            set
            {
                Policy = value;
            }
        }
    }
}
