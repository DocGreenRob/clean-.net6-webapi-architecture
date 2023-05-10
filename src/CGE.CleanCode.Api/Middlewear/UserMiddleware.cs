using Cge.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CGE.CleanCode.Api.Middlewear
{
	public class UserMiddleware
	{
		private readonly RequestDelegate _next;
		private IApplicationContext _applicationContext;

		public UserMiddleware(RequestDelegate next, IApplicationContext applicationContext)
		{
			_next = next;
			_applicationContext = applicationContext;

		}

		public async Task Invoke(HttpContext context)
		{
			//_applicationContext.Add(new User { Id = 1 });
			//var x = _applicationContext.Get<User>();
			await _next.Invoke(context);
		}
	}
}
