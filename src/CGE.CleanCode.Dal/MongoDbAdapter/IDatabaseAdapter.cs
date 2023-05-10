using CGE.CleanCode.Dal.MongoDbAdapter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CGE.CleanCode.Dal.MongoDbAdapter
{
	/// <summary>
	/// MongoDB adapter for database access
	/// </summary>
	public interface IDatabaseAdapter
	{
		/// <summary>
		/// Collection IQueryable for one-off non adapter methods
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <returns>An <see cref="IQueryable{T}"/></returns>
		IQueryable<T> CollectionQuery<T>();

		/// <summary>
		/// Delete many documents by a filter
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <param name="filter">The filter expression</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>True / False upon request or fail</returns>
		Task<bool> DeleteAllAsync<T>(
		  Expression<Func<T, bool>> filter,
		  CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete the first document found by a filter
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <param name="filter">The filter expression</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>True / False upon request or fail</returns>
		Task<bool> DeleteAsync<T>(
		 Expression<Func<T, bool>> filter,
		 CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete the first document by id
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <param name="id">The document id</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>True / False upon request or fail</returns>
		Task<bool> DeleteAsync<T>(
		  string id,
		  CancellationToken cancellationToken = default)
		  where T : class, IDatabaseDocument;

		/// <summary>
		/// Generates an unique ID
		/// </summary>
		/// <returns>Unique ID</returns>
		string GenerateNewId();

		/// <summary>
		/// Get documents by type
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <param name="filter">The filter expression</param>
		/// <param name="sort">the sort expression by property-key and sort-value</param>
		/// <param name="skip">The number of documents to skip</param>
		/// <param name="take">The number of documents to take</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>A <see cref="List{T}"/> of document types</returns>
		//Task<List<T>> GetAllAsync<T>(
		//	  Expression<Func<T, bool>> filterExpression = null,
		//	  Expression<Func<T, bool>> orderByExpression = null,
		//	  int skip = 0,
		//	  int take = 0,
		//	  CancellationToken cancellationToken = default);
		Task<List<T>> GetAllAsync<T>(
		  Expression<Func<T, bool>>? filterExpression = null,
		  Dictionary<string, object>? sort = null,
		  int skip = 0,
		  int take = 0,
		  CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the first document by the filter
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <param name="filter">The filter expression</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		Task<T> GetAsync<T>(
	  Expression<Func<T, bool>> filter,
	  CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the first document by the id
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <param name="id">The document id</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		Task<T> GetAsync<T>(
		  string id,
		  CancellationToken cancellationToken = default)
		  where T : class, IDatabaseDocument;

		/// <summary>
		/// Inserts a document
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <param name="document">The document to insert</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		Task InsertAsync<T>(
		  T document,
		  CancellationToken cancellationToken = default);

		Task<IQueryable<T>> SearchAsync<T>(
			Expression<Func<T, bool>> expression,
			IEnumerable<ExpressionMap<T>> regularExpressions = null,
			Dictionary<string, object> sort = null,
			int skip = 0,
			int take = 0,
			CancellationToken cancellationToken = default);
		/// <summary>
		/// Upserts a document by id
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <param name="id">The document id</param>
		/// <param name="updatedDocument">The new / updated document to insert</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		Task<T> UpsertAsync<T>(
		  string id,
		  T updatedDocument = null!,
		  CancellationToken cancellationToken = default)
		  where T : class, IDatabaseDocument;

		/// <summary>
		/// Upserts a document by a filter
		/// </summary>
		/// <typeparam name="T">The document type</typeparam>
		/// <param name="filter">The filter expression</param>
		/// <param name="updatedDocument">The new / updated document to insert</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		Task<T> UpsertAsync<T>(
		  Expression<Func<T, bool>> filter,
		  T updatedDocument = null!,
		  CancellationToken cancellationToken = default)
		  where T : class;
	}
}
