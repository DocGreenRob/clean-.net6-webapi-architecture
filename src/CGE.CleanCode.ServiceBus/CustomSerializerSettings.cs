using Newtonsoft.Json;

namespace CGE.CleanCode.ServiceBus
{
	public static class CustomSerializerSettings
	{
		public static JsonSerializerSettings GetSerializerSettings()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};
			settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
			return settings;
		}
	}
}