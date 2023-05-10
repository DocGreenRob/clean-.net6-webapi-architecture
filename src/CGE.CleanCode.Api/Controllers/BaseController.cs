using Cge.Core.Extensions;
using Cge.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CGE.CleanCode.Api.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	public class BaseController : ControllerBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="telemetryClient"></param>
		/// <param name="configuration"></param>
		public BaseController(ITelemetryClient telemetryClient, IConfiguration configuration)
		{
			TelemetryClient = telemetryClient.ValidateArgNotNull(nameof(telemetryClient));
			Configuration = configuration.ValidateArgNotNull(nameof(configuration));
		}

		/// <summary>
		/// 
		/// </summary>
		public IConfiguration Configuration { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ITelemetryClient TelemetryClient { get; set; }
	}
}
