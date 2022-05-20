using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities.Blocks;
using BitcoinRhExplorer.Library;
using System;
using System.Data.Entity;
using System.Linq;

namespace BitcoinRhExplorer.Server
{
    public class BaseAddressFromToRepository : BaseRepository<AddressFromTo>
    {

        public BaseAddressFromToRepository(IAmbientDbContextLocator ambientDbContextLocator) :
            base(ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null) throw new ArgumentNullException("ambientDbContextLocator");
        }

        public int DeleteByBlockId(long blockId)
        {
            var entites = DbContext.Set<AddressFromTo>()
                .AsQueryable()
                .Where(e => e.BlockId == blockId)
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
