using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BitcoinRhExplorer.EF.Interfaces 
{
    public interface IBaseRepository<TEntity>
    {
        TEntity GetById(long userId);
        IQueryable<TEntity> GetAll(bool includeDeleted = false);
        IQueryable<TEntity> GetByIds(IEnumerable<long> ids, bool includeDeleted = false);
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, bool includeDeleted = false);
        int Add(TEntity entity);
        int Update(TEntity entity);
        int Update(List<TEntity> entity);
        int Delete(TEntity entity);
    }
}
