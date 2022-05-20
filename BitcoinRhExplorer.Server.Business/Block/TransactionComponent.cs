using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities.Blocks;
using BitcoinRhExplorer.Library;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BitcoinRhExplorer.Server.Business.Block
{
    public class TransactionComponent : BaseDbComponent<Transaction>
    {
        public TransactionComponent(IDbContextScopeFactory dbContextScopeFactory, IAmbientDbContextLocator ambientDbContextLocator) :
            base(dbContextScopeFactory, ambientDbContextLocator)
        {
        }

        public Transaction GetByHash(string hash)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DbContext.Set<Transaction>()
                    .FirstOrDefault(e => (e.Hash == hash) && ((!e.IsDeleted) || (e.IsDeleted)));
            }
        }

        public Transaction GetByHashIndex(int hashIndex, string hash)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var list = _repository.DbContext.Set<Transaction>()
                    .Where(e => (e.HashIndex == hashIndex) && ((!e.IsDeleted) || (e.IsDeleted)))
                    .ToList();

                return list.Where(e => e.Hash == hash).FirstOrDefault();
            }
        }

        public void UpdateIndex()
        {
            IEnumerable<string> entitySets = null;

            using (_dbContextScopeFactory.Create())
            {
                var all = _repository.GetAll(true);

                foreach (var item in all)
                {

                    item.HashIndex = TextHelper.GetNumberHash(item.Hash);

                    _repository.DbContext.Entry(item).State = EntityState.Modified;
                }

                _repository.DbContext.SaveChanges();

                entitySets = ((IObjectContextAdapter)_repository.DbContext)
                    .ObjectContext
                    .MetadataWorkspace
                    .GetEntityContainer("CodeFirstDatabase", DataSpace.SSpace)
                    .EntitySets
                    .Select(e => e.Name);
            }

            base.ClearLevel2Cache(entitySets);
        }
    }
}
