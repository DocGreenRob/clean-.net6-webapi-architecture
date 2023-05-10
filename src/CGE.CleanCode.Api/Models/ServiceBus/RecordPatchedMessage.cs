namespace CGE.CleanCode.Api.Models.ServiceBus
{
	public class RecordPatchedMessage<T>
	{
		public T OldRecord { get; set; }
		public T NewRecord { get; set; }
		public string TypeName { get; set; }
	}
}
