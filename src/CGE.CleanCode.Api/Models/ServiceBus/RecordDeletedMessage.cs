namespace CGE.CleanCode.Api.Models.ServiceBus
{
	public class RecordDeletedMessage
	{
		public string EntityType { get; set; }
		public string Id { get; set; }
	}
}
