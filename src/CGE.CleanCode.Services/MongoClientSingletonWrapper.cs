using Cge.Core.Extensions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Security.Authentication;


namespace CGE.CleanCode.Service
{
	/// <summary>
	/// Used as a workaround for DI
	/// I have a class that can't support DI but I need a property from something that was injected
	/// Here is a workaround
	/// (inspired by CGE.CleanCode.Service.MongoClientSingletonWrapper)
	/// </summary>
	public class SingletonFactoryWrapper
	{
		private readonly Dictionary<string, object> _dictionaryOfKeys;
		private readonly IConfiguration _configuration;

		public SingletonFactoryWrapper(IConfiguration configuration)
		{
			_configuration = configuration.ValidateArgNotNull(nameof(configuration)); 

            _dictionaryOfKeys = new Dictionary<string, object>();
			_dictionaryOfKeys.Add(nameof(configuration), _configuration);
		}

		public object GetThing(string key)
		{
			_dictionaryOfKeys.TryGetValue(key, out object thing);

			return thing;
		}

		public void AddThing(string key, object value)
		{
			_dictionaryOfKeys.Add(key, value);
		}

		public void AddToConfiguration(string key, string value)
		{
			var x = (IConfiguration)GetThing(key);
			x[key] = value;

			_dictionaryOfKeys.Remove(key);
			_dictionaryOfKeys.Add("configuration", x);
		}
	}

	public class MongoClientSingletonWrapper
	{
		public MongoClient MongoClient { get; set; }
		private static MongoClientSingletonWrapper instance = null;
		private static readonly object padlock = new object();
		private readonly IConfiguration _configuration;

		public MongoClientSingletonWrapper()
		{
			
		}

		public static MongoClientSingletonWrapper Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
					{
						instance = new MongoClientSingletonWrapper();

						var mongoClient = default(MongoClient);

						string connectionString = "CONNECTION_STRING";
						MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
						settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

						mongoClient = new MongoClient(settings);

						instance.MongoClient = mongoClient;
					}

					return instance;
				}
			}
		}
	}
}
