using CGE.CleanCode.Dal.MongoDbAdapter;

namespace CGE.CleanCode.Service
{
	public interface IMongoDbWrapper
    {
        IDatabaseAdapter GetDatabaseAdapter();
    }
}
