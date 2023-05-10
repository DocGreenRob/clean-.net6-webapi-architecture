namespace CGE.CleanCode.Api.Models.Cache
{
	public class NewRecordDto
	{
		public string Key { get; set; } = string.Empty;
		public object Value { get; set; } = null;
		public string Type { get; set; } = string.Empty;
	}
}
