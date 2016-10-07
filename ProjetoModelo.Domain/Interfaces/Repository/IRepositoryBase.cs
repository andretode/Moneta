using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moneta.Domain.Interfaces.Repository
{
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : class 
    {
        void Add(TEntity obj);
        TEntity GetById(Guid id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetAllReadOnly();
        void Update(TEntity obj);
        void Remove(TEntity obj);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}
