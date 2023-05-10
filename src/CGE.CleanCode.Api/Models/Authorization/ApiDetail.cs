using System;

namespace CGE.CleanCode.Api.Models.Authorization
{
    public class ApiDetail
    {
        public ApiDetail(string url, params string[] httpVerbs)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            HttpVerbs = httpVerbs ?? throw new ArgumentNullException(nameof(httpVerbs));
        }

        public string[] HttpVerbs { get; set; }
        public string Url { get; set; }
    }
}
