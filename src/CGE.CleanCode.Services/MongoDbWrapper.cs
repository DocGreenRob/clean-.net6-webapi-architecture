using Cge.Core.Extensions;
using Cge.Core.Logging;
using Microsoft.Extensions.Configuration;
using CGE.CleanCode.Common;
using CGE.CleanCode.Dal.MongoDbAdapter;
using System;
using System.Collections.Generic;

namespace CGE.CleanCode.Service
{
	public class MongoDbWrapper<T> : IMongoDbWrapper where T : class
	{
		private readonly ITelemetryClient _telemetryClient;
		private readonly IConfiguration _configuration;
		private readonly IDatabaseAdapter _databaseAdapter;
		private readonly Dictionary<string, IDatabaseAdapter> _adapters;


		public MongoDbWrapper(IConfiguration configuration,
								ITelemetryClient telemetryClient,
								string documentName)
		{
			// TODO: add logging

			// Validate stuff
			_configuration = configuration.ValidateArgNotNull(nameof(configuration));
			_configuration[Globals.ConfigurationKeys.Database].ValidateArgNotNull(Globals.ConfigurationKeys.Database);
			_telemetryClient = telemetryClient.ValidateArgNotNull(nameof(telemetryClient));
			documentName.ValidateArgNotNull(nameof(documentName));

			// Set values from configuration
			var database = _configuration[Globals.ConfigurationKeys.Database];

			// Create/Connect to MongoDb
			var dictionary = new Dictionary<Type, (string, string)> { { typeof(T), (database, documentName) } };
			var mongoClient = MongoClientSingletonWrapper.Instance(_configuration).MongoClient;

			if (_adapters == null)
			{
				_adapters = new Dictionary<string, IDatabaseAdapter>();
			}

			if (!_adapters.TryGetValue(documentName, out IDatabaseAdapter databaseAdapter))
			{
				_databaseAdapter = new MongoDBAdapter(mongoClient, dictionary);

				if (_adapters == null
					|| _adapters == default(Dictionary<string, IDatabaseAdapter>)
					|| _adapters.Count == 0)
				{
					_adapters = new Dictionary<string, IDatabaseAdapter>();
				}

				_adapters.Add(documentName, _databaseAdapter);
			}
			else
			{
				_databaseAdapter = databaseAdapter;
			}

		}

		public IDatabaseAdapter GetDatabaseAdapter()
		{
			return _databaseAdapter;
		}
	}
}
