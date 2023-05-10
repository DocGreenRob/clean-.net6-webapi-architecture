using Cge.Core.Extensions;
using Cge.Core.Logging;
using Microsoft.Extensions.Configuration;
using CGE.CleanCode.Dal.MongoDbAdapter;

namespace CGE.CleanCode.Service.Services
{
	public abstract class BaseService<T> where T : class
	{
		public readonly IDatabaseAdapter DatabaseAdapter;
		public readonly ITelemetryClient TelemetryClient;
		public readonly IConfiguration Configuration;

		public BaseService(IMongoDbWrapper mongoDbWrapper,
							IConfiguration configuration,
							ITelemetryClient telemetryClient,
							string documentName)
		{
			// Validate stuff
			Configuration = configuration.ValidateArgNotNull(nameof(configuration));
			TelemetryClient = telemetryClient.ValidateArgNotNull(nameof(telemetryClient));
			mongoDbWrapper.ValidateArgNotNull(nameof(mongoDbWrapper));

			var _ = new MongoDbWrapper<T>(configuration, telemetryClient, documentName);
			DatabaseAdapter = _.GetDatabaseAdapter();
		}
	}
}
