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
        private readonly SchoolContext _ctx;
        private Dictionary<Type, object> _repositories;
        private bool _disposed;

        public UnitOfWork(SchoolContext context)
        {
            _ctx = context;
            _repositories = new Dictionary<Type, object>();
            _disposed = false;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories.Keys.Contains(typeof(TEntity)))
                return _repositories[typeof(TEntity)] as IRepository<TEntity>;

            var repository = new Repository<TEntity>(_ctx);
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
                    _ctx.Dispose();
                }

                this._disposed = true;
            }
        }

        public void Commit()
        {
            _ctx.SaveChanges();
        }

    }
}
