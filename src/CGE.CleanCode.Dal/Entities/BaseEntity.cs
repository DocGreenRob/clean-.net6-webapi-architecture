using CGE.CleanCode.Dal.MongoDbAdapter;
using System;

namespace CGE.CleanCode.Dal.Entities
{
	public abstract class BaseEntity : IDatabaseDocument
	{
		public string CreatedBy { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; } = string.Empty;
		public DateTime UpdatedDate { get; set; }
		public bool IsDeleted { get; set; }
		public string Id { get; set; } = string.Empty;
	}
}
