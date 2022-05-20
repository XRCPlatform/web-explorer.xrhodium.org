using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities;
using BitcoinRhExplorer.Library;
using System.Linq.Expressions;

namespace BitcoinRhExplorer.Server
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : RichEntity, IIdEntity
    {
		private readonly IAmbientDbContextLocator _ambientDbContextLocator;

		public BitcoinRhExplorerDbContext DbContext
		{
			get
			{
				var dbContext = _ambientDbContextLocator.Get<BitcoinRhExplorerDbContext>();

				if (dbContext == null)
					throw new InvalidOperationException("No ambient DbContext of type UserManagementDbContext found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");
				
				return dbContext;
			}
		}

        public BaseRepository(IAmbientDbContextLocator ambientDbContextLocator)
		{
			if (ambientDbContextLocator == null) throw new ArgumentNullException("ambientDbContextLocator");
			_ambientDbContextLocator = ambientDbContextLocator;
		}

        public virtual TEntity GetById(long id)
		{
            return DbContext.Set<TEntity>()
                .FirstOrDefault(e => (e.Id == id) && ((!e.IsDeleted) || (e.IsDeleted)));
		}

      public virtual IQueryable<TEntity> GetAll(bool includeDeleted = false)
        {
            return DbContext.Set<TEntity>()
                .Where(e => (!e.IsDeleted) || (e.IsDeleted && includeDeleted));
        }

     public virtual IQueryable<TEntity> GetByIds(IEnumerable<long> ids, bool includeDeleted = false)
        {
            return DbContext.Set<TEntity>().AsQueryable()
                .Where(e => ids.Contains(e.Id) && ((!e.IsDeleted) || (e.IsDeleted && includeDeleted)));
        }

        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate,
            bool includeDeleted = false)
        {
            return DbContext.Set<TEntity>()
                .Where(predicate).Where(e => (!e.IsDeleted) || (e.IsDeleted && includeDeleted));
        }

        public virtual int Add(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Added;
            return DbContext.SaveChanges();
        }
        public virtual int Add(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbContext.Entry(entity).State = EntityState.Added;
            }

            return DbContext.SaveChanges();
        }

        public virtual int Update(TEntity entity)
        {
            entity.IfDefined(richEntity =>
            {
                richEntity.UpdatedUtc = DateTime.UtcNow;
            });

            DbContext.Entry(entity).State = EntityState.Modified;
            return DbContext.SaveChanges();
        }

        public virtual int Update(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.IfDefined(richEntity =>
                {
                    richEntity.UpdatedUtc = DateTime.UtcNow;
                });

                DbContext.Entry(entity).State = EntityState.Modified;
            }

            return DbContext.SaveChanges();
        }
        public virtual int Delete(TEntity entity)
        {
            entity.IfDefined(richEntity =>
            {
                richEntity.UpdatedUtc = DateTime.UtcNow;
                richEntity.IsDeleted = true;
            });

            DbContext.Entry(entity).State = EntityState.Modified;
            return DbContext.SaveChanges();
        }
        public virtual int Delete(List<long> ids)
        {
            var entites = DbContext.Set<TEntity>()
                .AsQueryable()
                .Where(e => ids.Contains(e.Id))
                .ToList();

            foreach (var entity in entites)
            {
                entity.IfDefined(richEntity =>
                {
                    richEntity.UpdatedUtc = DateTime.UtcNow;
                    richEntity.IsDeleted = true;
                });

                DbContext.Entry(entity).State = EntityState.Modified;
            }

            return DbContext.SaveChanges();
        }
    }
}
