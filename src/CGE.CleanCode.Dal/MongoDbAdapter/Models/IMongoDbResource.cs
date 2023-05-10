using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CGE.CleanCode.Dal.MongoDbAdapter.Models
{
	public interface IMongoDbResource
	{
		[BsonId]
		ObjectId _id { get; set; }
	}
}
