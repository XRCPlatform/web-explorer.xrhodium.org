using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities.Blocks;
using System;
using System.Data.Entity;
using System.Linq;

namespace BitcoinRhExplorer.Server
{
    public class BaseTransactionRepository : BaseRepository<Transaction>
    {

        public BaseTransactionRepository(IAmbientDbContextLocator ambientDbContextLocator) :
            base(ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null) throw new ArgumentNullException("ambientDbContextLocator");
        }

        public int DeleteByBlockId(long blockId)
        {
            var entites = DbContext.Set<Transaction>()
                .AsQueryable()
                .Where(e => e.BlockId == blockId)
                .ToList();

            foreach (var entity in entites)
            {
                DbContext.Entry(entity).State = EntityState.Deleted;
            }

            return DbContext.SaveChanges();
        }
    }
}
