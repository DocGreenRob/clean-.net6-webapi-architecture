using System;
using System.Linq.Expressions;

namespace CGE.CleanCode.Dal.MongoDbAdapter.Models
{
	public class ExpressionMap<T>
	{
		public string RegEx { get; set; }
		public Expression<Func<T, object>> TargetPropertySelector { get; set; }
	}
}
