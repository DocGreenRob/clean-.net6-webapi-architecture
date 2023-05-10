using CGE.CleanCode.Dal.Entities;
using System;

namespace CGE.CleanCode.Dal.Extensions
{
	public static class EntityExtensions
	{
		public static void SetAuditDefaults<T>(this T instance) where T : BaseEntity
		{
			instance.IsDeleted = false;
			instance.CreatedBy = "updated-by-user";
			instance.CreatedDate = DateTime.UtcNow;
			instance.UpdatedBy = "updated-by-user";
			instance.UpdatedDate = DateTime.UtcNow;
		}
	}
}
