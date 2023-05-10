using Cge.Core.Extensions;
using Cge.Core.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace CGE.CleanCode.Api.Middlewear
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ITelemetryClient _telemetryClient;

		public ExceptionMiddleware(RequestDelegate next, ITelemetryClient telemetryClient)
		{
			_next = next;
			_telemetryClient = telemetryClient.ValidateArgNotNull(nameof(telemetryClient));
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next.Invoke(context);
			}
			catch (Exception ex)
			{
				string result = JsonConvert.SerializeObject(new { Message = $"{ex.Message} {(ex.InnerException != null ? ex.InnerException.Message : "")}" });

				if (ex is ValidationException)
				{
					context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				}
				else
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				}

				_telemetryClient.TrackException(ex);
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync(result);
			}
		}
	}
}
