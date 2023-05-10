namespace CGE.CleanCode.Api.Models
{
	public class ErrorResponseDto
	{
		public string Description { get; set; } = string.Empty;
		public int ErrorCode { get; set; }
	}
}
