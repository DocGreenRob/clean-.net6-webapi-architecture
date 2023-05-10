using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using CGE.CleanCode.Dal.MongoDbAdapter.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CGE.CleanCode.Dal.MongoDbAdapter
{
	/// <inheritdoc cref="IDatabaseAdapter"/> 
	public class MongoDBAdapter : IDatabaseAdapter
	{
		internal readonly ConcurrentDictionary<Type, (IMongoDatabase Database, string CollectionName)> datebaseCollectionsByType;

		public MongoDBAdapter(IMongoClient mongoClient,
								Dictionary<Type, (string DatabaseName, string CollectionName)> datebaseCollectionsByType)
		{
			this.datebaseCollectionsByType = new ConcurrentDictionary<Type, (IMongoDatabase, string)>(datebaseCollectionsByType.ToDictionary(x => x.Key, x => (mongoClient.GetDatabase(x.Value.DatabaseName), x.Value.CollectionName)));
		}

		public IQueryable<T> CollectionQuery<T>() => Collection<T>().AsQueryable();

		public async Task<bool> DeleteAllAsync<T>(
		  Expression<Func<T, bool>> filter,
		  CancellationToken cancellationToken = default)
		{
			var deleteResult = await Collection<T>().DeleteManyAsync<T>(filter, cancellationToken);

			return deleteResult.DeletedCount > 0;
		}

		public async Task<bool> DeleteAsync<T>(
		  Expression<Func<T, bool>> filter,
		  CancellationToken cancellationToken = default)
		{
			var deleteResult = await Collection<T>().DeleteOneAsync(filter, cancellationToken);

			return deleteResult.DeletedCount > 0;
		}

		public async Task<bool> DeleteAsync<T>(
		  string id,
		  CancellationToken cancellationToken = default)
		  where T : class, IDatabaseDocument
		{
			return await DeleteAsync<T>(document => document.Id == id, cancellationToken);
		}

		public string GenerateNewId()
		{
			return ObjectId.GenerateNewId().ToString();
		}

		public async Task<List<T>> GetAllAsync<T>(Expression<Func<T, bool>>? filterExpression = null,
													Dictionary<string, object>? sort = null,
													int skip = 0,
													int take = 0,
													CancellationToken cancellationToken = default)
		{
			var options = new FindOptions<T>()
			{
				Limit = take,
				Skip = skip,
				Sort = null == sort ? null : new BsonDocument(sort)
			};

			var results = await Collection<T>().FindAsync(filterExpression, options, cancellationToken);

			return await results.ToListAsync(cancellationToken);
		}

		public async Task<T> GetAsync<T>(
		  Expression<Func<T, bool>> filterExpression,
		  CancellationToken cancellationToken = default)
		{
			if (filterExpression == null)
			{
				throw new ArgumentNullException(nameof(filterExpression));
			}

			// broke
			var results = await Collection<T>().FindAsync(filterExpression, default, cancellationToken);
			return await results.FirstOrDefaultAsync(cancellationToken);
			// fixed
			//var a = Collection<T>();
			//var b = a.AsQueryable();
			//return await Task.FromResult(b.Where(filterExpression).ToList().FirstOrDefault());
		}

		public async Task<T> GetAsync<T>(
		  string id,
		  CancellationToken cancellationToken = default)
		  where T : class, IDatabaseDocument
		{
			return await GetAsync<T>(document => document.Id == id, cancellationToken);
		}

		public async Task InsertAsync<T>(
		  T document,
		  CancellationToken cancellationToken = default)
		{
			// broke
			await Collection<T>().InsertOneAsync(document, default, cancellationToken);

			// fixed
			//Collection<T>().InsertOne(document);
			// theory 1
			//await Collection<T>().InsertOneAsync(document, cancellationToken).ConfigureAwait(false);
			// failed... now about to try web api project to see what's going on... again 
			// the app just suspended and closed...
		}

		public async Task<IQueryable<T>> SearchAsync<T>(
			Expression<Func<T, bool>> expression,
			IEnumerable<ExpressionMap<T>> regularExpressions = null,
			Dictionary<string, object> sort = null,
			int skip = 0,
			int take = 0,
			CancellationToken cancellationToken = default)
		{
			var options = new FindOptions<T>()
			{
				Limit = take,
				Skip = skip,
				Sort = null == sort ? null : new BsonDocument(sort)
			};

			var query = await GetAllAsync(expression, sort, skip, take, cancellationToken).ConfigureAwait(false);
			var masterList = new List<T>();

			if (regularExpressions != null)
			{
				foreach (var regEx in regularExpressions)
				{
					var queryResults = from row in query.AsQueryable()
									   let matches = new Regex(regEx.RegEx).Matches(GetValue(row, regEx.TargetPropertySelector).ToString())
									   where matches.Count > 0
									   select row;
					masterList.AddRange(queryResults);
				}
			}
			return masterList.AsQueryable();
		}

		public async Task<T> UpsertAsync<T>(
		  string id,
		  T updatedDocument = null!,
		  CancellationToken cancellationToken = default)
		  where T : class, IDatabaseDocument
		{
			return await UpsertAsync(document => document.Id == id, updatedDocument, cancellationToken);
		}

		public async Task<T> UpsertAsync<T>(Expression<Func<T, bool>> filter,
											  T updatedDocument = null!,
											  CancellationToken cancellationToken = default)
											  where T : class
		{
			var mongoOptions = new FindOneAndUpdateOptions<T, T>
			{
				IsUpsert = true,
				ReturnDocument = ReturnDocument.After
			};

			return await Collection<T>().FindOneAndUpdateAsync(filter, updatedDocument.ToJson(), mongoOptions, cancellationToken);
		}

		internal IMongoCollection<T> Collection<T>()
		{
			var (Database, CollectionName) = datebaseCollectionsByType[typeof(T)];
			return Database.GetCollection<T>(CollectionName);
		}

		private PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> propertyLambda)
		{
			Type type = typeof(T);

			MemberExpression member = propertyLambda.Body as MemberExpression;
			if (member == null)
				throw new ArgumentException(string.Format(
					"Expression '{0}' refers to a method, not a property.",
					propertyLambda.ToString()));

			PropertyInfo propInfo = member.Member as PropertyInfo;
			if (propInfo == null)
				throw new ArgumentException(string.Format(
					"Expression '{0}' refers to a field, not a property.",
					propertyLambda.ToString()));

			if (type != propInfo.ReflectedType &&
				!type.IsSubclassOf(propInfo.ReflectedType))
				throw new ArgumentException(string.Format(
					"Expression '{0}' refers to a property that is not from type {1}.",
					propertyLambda.ToString(),
					type));

			return propInfo;
		}

		private object? GetValue<T>(T target, Expression<Func<T, object>> selector)
		{
			return target.GetType().GetProperty(GetPropertyInfo(selector).Name).GetValue(target, null);
		}
	}
}
