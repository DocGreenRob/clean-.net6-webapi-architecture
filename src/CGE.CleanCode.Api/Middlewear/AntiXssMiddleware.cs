using Cge.Core.Extensions;
using Cge.Core.Logging;
using Microsoft.AspNetCore.Http;
using CGE.CleanCode.Api.Models;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CGE.CleanCode.Api.Middlewear
{
	public class AntiXssMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly int _statusCode = (int)HttpStatusCode.BadRequest;
		private readonly ITelemetryClient _telemetryClient;
		private ErrorResponseDto _error;
		private string _requestUrl;
		private string _requestQueryString;
		private string _requestBody;

		public AntiXssMiddleware(RequestDelegate next, ITelemetryClient telemetryClient)
		{
			_next = next.ValidateArgNotNull(nameof(next));
			_telemetryClient = telemetryClient.ValidateArgNotNull(nameof(telemetryClient));
		}

		public async Task Invoke(HttpContext context)
		{
			_requestUrl = context.Request.Path.Value;
			_requestQueryString = context.Request.QueryString.Value;

			// Check XSS in URL
			if (!string.IsNullOrWhiteSpace(context.Request.Path.Value))
			{
				var url = context.Request.Path.Value;

				if (CrossSiteScriptingValidation.IsDangerousString(url, out _))
				{
					await RespondWithAnError(context).ConfigureAwait(false);
					return;
				}
			}

			// Check XSS in query string
			if (!string.IsNullOrWhiteSpace(context.Request.QueryString.Value))
			{
				var queryString = WebUtility.UrlDecode(context.Request.QueryString.Value);

				if (CrossSiteScriptingValidation.IsDangerousString(queryString, out _))
				{
					await RespondWithAnError(context).ConfigureAwait(false);
					return;
				}
			}

			// Check XSS in request content
			// TODO: Validate against:
			// ' OR '1'='1' (' OR ') I can check this string for varitions of the attack
			var originalBody = context.Request.Body;
			try
			{
				var content = await ReadRequestBody(context);
				_requestUrl = content;

				if (CrossSiteScriptingValidation.IsDangerousString(content, out _))
				{
					await RespondWithAnError(context).ConfigureAwait(false);
					return;
				}
				await _next(context).ConfigureAwait(false);
			}
			finally
			{
				context.Request.Body = originalBody;
			}
		}

		private static async Task<string> ReadRequestBody(HttpContext context)
		{
			var buffer = new MemoryStream();
			await context.Request.Body.CopyToAsync(buffer);
			context.Request.Body = buffer;
			buffer.Position = 0;

			var encoding = Encoding.UTF8;

			var requestContent = await new StreamReader(buffer, encoding).ReadToEndAsync();
			context.Request.Body.Position = 0;

			return requestContent;
		}

		private async Task RespondWithAnError(HttpContext context)
		{
			context.Response.Clear();
			context.Response.Headers.AddHeaders();
			context.Response.ContentType = "application/json; charset=utf-8";
			context.Response.StatusCode = _statusCode;

			if (_error == null)
			{
				_error = new ErrorResponseDto
				{
					Description = "Error from AntiXssMiddleware",
					ErrorCode = 500
				};
			}

			var requestProperties = new Dictionary<string, string>();
			requestProperties.Add("RequestUrl", _requestUrl);
			requestProperties.Add("RequestQueryString", _requestQueryString);
			requestProperties.Add("RequestBody", _requestBody);

			_telemetryClient.TrackEvent("XssAttempt", requestProperties);

			await context.Response.WriteAsync(_error.ToJSON());
		}
	}
}

