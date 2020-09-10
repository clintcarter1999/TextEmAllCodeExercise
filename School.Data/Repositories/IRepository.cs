using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using School.Data.Context;

namespace School.Data.Repositories
{
    /// <summary>
    /// Use as the base repository interface for each Model's Repository Interface.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        SchoolContext DataBaseContext { get; }

        /// <summary>
        /// Get a single item from the repository by Id
        /// </summary>
        /// <param name="id">Id of the item in the repository/table</param>
        /// <returns>TEntity</returns>
        Task<TEntity> GetItemAsync(Guid id);

        Task<TEntity> GetItemAsync(int id);

        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Asynchronously Get all items in the respository for the given type TEntity
        /// </summary>
        /// <returns>IEnumerable of TEntity</returns>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// Find an item in the repository
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        RepositoryStatus Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        void SetState(TEntity entity, EntityState modified);

        Task<int> Commit();
    }

    /// <summary>
    /// There are some cases where there are multiple failure modes.
    /// This class provides a standard return package indicating success/error and a message.
    /// </summary>
    public class RepositoryStatus
    {
        /// <summary>
        /// Returns True if successful.  False if not successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Holds the message if applicable.
        /// Mostly used to hold the error message
        /// </summary>
        public string Message { get; set; }

        public RepositoryStatus(bool isSuccess, string message)
        {
            Success = isSuccess;
            Message = message;
        }
    }
}
