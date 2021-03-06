﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using School.Data.Context;

namespace School.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<TEntity> dbEntity;

        public Repository(DbContext context)
        {
            Context = context;

            if (context != null)
                dbEntity = context.Set<TEntity>();
        }

        public async Task<TEntity> GetItemAsync(Guid id)
        {
            return await dbEntity.FindAsync(id);
        }

        public async Task<TEntity> GetItemAsync(int id)
        {
            return await dbEntity.FindAsync(id);
        }

        /// <summary>
        /// Temporary non-async version to test Mock
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll()
        {
            return dbEntity.ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbEntity.ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return dbEntity.Where(predicate);
        }

        /// <summary>
        /// Returns the count of items held by the TEntity object
        /// </summary>
        public int Count
        {
            get { return Context.Set<TEntity>().Count(); }
        }

        public SchoolContext DataBaseContext
        {
            get { return Context as SchoolContext; }
        }

        /// <summary>
        /// NOTE: I do not advocate using Tuple as a return type.
        /// I would prefer to use a Class
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RepositoryStatus Add(TEntity entity)
        {
            bool success = false;
            string msg = "Success";

            if (!dbEntity.Contains<TEntity>(entity))
            {
                dbEntity.Add(entity);

                success = true;
            }
            else
            {
                msg = "The item already exists. Choose a unique id or use different API to modify the existing item.";
            }

            return new RepositoryStatus(success, msg);
        }

        /// <summary>
        /// Adds a Range of TEntity.  Mostly used for UnitTesting.
        /// </summary>
        /// <param name="entities">IEnumerable of TEntity</param>
        public void AddRange(IEnumerable<TEntity> entities)
        {
            dbEntity.AddRange(entities);
        }

        /// <summary>
        /// Removes an item from the Repository
        /// </summary>
        /// <param name="entity">TEntity</param>
        public void Remove(TEntity entity)
        {
            dbEntity.Remove(entity);
        }

        /// <summary>
        /// Removes a range of items from the Repository
        /// </summary>
        /// <param name="entities"></param>
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            dbEntity.RemoveRange(entities);
        }

        /// <summary>
        /// Set the State of the Entity (Modified, Saved, Deleted, etc)
        /// </summary>
        /// <param name="item">TEntity</param>
        /// <param name="state">EntityState</param>
        public void SetState(TEntity item, EntityState state)
        {
            // Implementing in this derived class.  Seem's generic enough
            Context.Entry(item).State = state;
        }

        public async Task<int> Commit()
        {
            //Once I add other IRepository derived Interfaces we can implement one save here
            return await Context.SaveChangesAsync();
        }
    }
}
