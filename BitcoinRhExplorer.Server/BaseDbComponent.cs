using System;
using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace BitcoinRhExplorer.Server
{
    public class BaseDbComponent<TEntity> where TEntity : RichEntity, IIdEntity
    {
        protected readonly IDbContextScopeFactory _dbContextScopeFactory;
        protected readonly IAmbientDbContextLocator _ambientDbContextLocator;
        protected readonly BaseRepository<TEntity> _repository;

        public BaseDbComponent(IDbContextScopeFactory dbContextScopeFactory, IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (dbContextScopeFactory == null) throw new ArgumentNullException("dbContextScopeFactory");
            if (ambientDbContextLocator == null) throw new ArgumentNullException("ambientDbContextLocator");
            _dbContextScopeFactory = dbContextScopeFactory;
            _ambientDbContextLocator = ambientDbContextLocator;
            _repository = new BaseRepository<TEntity>(ambientDbContextLocator);
        }

        public virtual TEntity Create()
        {
            var entity = (TEntity)typeof(TEntity).GetConstructor(new Type[0]).Invoke(new object[0]);

            return entity;
        }

        public virtual TEntity GetById(long id)
		{
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.GetById(id);
            }
		}

       public virtual List<TEntity> GetAll(bool includeDeleted = false)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.GetAll(includeDeleted)
                    .ToList();
            }
        }

        public virtual List<TEntity> GetByIds(IEnumerable<long> ids, bool includeDeleted = false)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.GetByIds(ids, includeDeleted)
                    .ToList();
            }
        }

        public virtual List<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate,
            bool includeDeleted = false)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.FindBy(predicate, includeDeleted)
                    .ToList();
            }
        }

       public virtual int Add(TEntity entity)
        {
            var state = 0;
            IEnumerable<string> entitySets = null;

            using (_dbContextScopeFactory.Create())
            {
                state = _repository.Add(entity);

                entitySets = ((IObjectContextAdapter)_repository.DbContext)
                    .ObjectContext
                    .MetadataWorkspace
                    .GetEntityContainer("CodeFirstDatabase", DataSpace.SSpace)
                    .EntitySets
                    .Select(e => e.Name);
            }

            ClearLevel2Cache(entitySets);

            return state;
        }
        public virtual int Add(List<TEntity> entities)
        {
            var state = 0;
            IEnumerable<string> entitySets = null;

            using (_dbContextScopeFactory.Create())
            {
                state = _repository.Add(entities);
                entitySets = ((IObjectContextAdapter)_repository.DbContext)
                    .ObjectContext
                    .MetadataWorkspace
                    .GetEntityContainer("CodeFirstDatabase", DataSpace.SSpace)
                    .EntitySets
                    .Select(e => e.Name);
            }

            ClearLevel2Cache(entitySets);

            return state;
        }

        public virtual int Update(TEntity entity)
        {
            var state = 0;
            IEnumerable<string> entitySets = null;

            using (_dbContextScopeFactory.Create())
            {
                state = _repository.Update(entity);

                entitySets = ((IObjectContextAdapter)_repository.DbContext)
                    .ObjectContext
                    .MetadataWorkspace
                    .GetEntityContainer("CodeFirstDatabase", DataSpace.SSpace)
                    .EntitySets
                    .Select(e => e.Name);
            }

            ClearLevel2Cache(entitySets);

            return state;
        }

        public virtual int Update(List<TEntity> entities)
        {
            var state = 0;
            IEnumerable<string> entitySets = null;

            using (_dbContextScopeFactory.Create())
            {
                state = _repository.Update(entities);
                entitySets = ((IObjectContextAdapter)_repository.DbContext)
                    .ObjectContext
                    .MetadataWorkspace
                    .GetEntityContainer("CodeFirstDatabase", DataSpace.SSpace)
                    .EntitySets
                    .Select(e => e.Name);
            }

            ClearLevel2Cache(entitySets);

            return state;
        }

        public virtual int Delete(TEntity entity)
        {
            var state = 0;
            IEnumerable<string> entitySets = null;

            using (_dbContextScopeFactory.Create())
            {
                state = _repository.Delete(entity);

                entitySets = ((IObjectContextAdapter)_repository.DbContext)
                    .ObjectContext
                    .MetadataWorkspace
                    .GetEntityContainer("CodeFirstDatabase", DataSpace.SSpace)
                    .EntitySets
                    .Select(e => e.Name);
            }

            ClearLevel2Cache(entitySets);

            return state;
        }

        public void ClearLevel2Cache(IEnumerable<string> entitySets)
        {
            if (entitySets != null)
            {
                BitcoinRhExplorerDbCacheConfiguration.EntityFrameworkLevel2Cache.InvalidateSets(entitySets);
                BitcoinRhExplorerDbCacheConfiguration.EntityFrameworkLevel2Cache.Purge();
            }
        }
    }
}
