using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using School.Data.Context;

namespace School.Data.Repositories
{
    /// <summary>
    /// This UnitOfWork implementation "sucks" Repositories INTO it's ownership via the GetRepository method
    /// and stored them in a Dictionary.  Each of these repositories are injected with the same SchoolContext.
    /// Therefore, SaveChanges acts like a transaction against all changes tracked by the DbContext/SchoolContext.
    ///
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private Dictionary<Type, object> _repositories;
        private bool _disposed;

        public SchoolContext Context { get; set; }

        public UnitOfWork(SchoolContext context)
        {
            this.Context = context;

            _repositories = new Dictionary<Type, object>();
            _disposed = false;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories.Keys.Contains(typeof(TEntity)))
                return _repositories[typeof(TEntity)] as IRepository<TEntity>;

            var repository = new Repository<TEntity>(this.Context);
            _repositories.Add(typeof(TEntity), repository);
            return repository;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                }

                this._disposed = true;
            }
        }

        public void Commit()
        {
            this.Context.SaveChanges();
        }

    }
}
