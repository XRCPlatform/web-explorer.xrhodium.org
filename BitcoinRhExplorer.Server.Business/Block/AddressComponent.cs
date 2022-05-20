using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities.Blocks;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BitcoinRhExplorer.Server.Business.Block
{
    public class AddressComponent : BaseDbComponent<AddressFromTo>
    {
        public AddressComponent(IDbContextScopeFactory dbContextScopeFactory, IAmbientDbContextLocator ambientDbContextLocator) :
            base(dbContextScopeFactory, ambientDbContextLocator)
        {
        }

        public List<AddressFromTo> GetByAddress(string address)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DbContext.Set<AddressFromTo>()
                    .Where(e => (e.Address == address) && ((!e.IsDeleted) || (e.IsDeleted)))
                    .ToList();
            }
        }

        public int GetByAddressCount(int addressIndex, string address)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var list = _repository.DbContext.Set<AddressFromTo>()
                    .Where(e => (e.AddressIndex == addressIndex) && ((!e.IsDeleted) || (e.IsDeleted)))
                    .ToList();

                return list.Select(s => s.BlockId).Distinct().Count();
            }
        }

        public List<long> GetByAddressIndex(int addressIndex, string address, int offset)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var list = _repository.DbContext.Set<AddressFromTo>()
                    .Where(e => (e.AddressIndex == addressIndex) && ((!e.IsDeleted) || (e.IsDeleted)))
                    .OrderByDescending(e => e.BlockId)
                    .ToList();

                return list.Select(s => s.BlockId).Distinct().Skip(offset).Take(20).ToList();
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
                    _repository.DbContext.Entry(item).State = EntityState.Deleted;
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
