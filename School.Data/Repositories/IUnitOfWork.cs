using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using School.Data.Context;

namespace School.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        SchoolContext Context { get; set; }

        void Commit();
    }

    //public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    //{
    //    TContext Context { get; }
    //}
}
