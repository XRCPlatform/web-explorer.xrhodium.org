using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities.Stats;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinRhExplorer.Server.Business.Stats
{
    public class RichStatComponent : BaseDbComponent<RichStat>
    {
        public RichStatComponent(IDbContextScopeFactory dbContextScopeFactory, IAmbientDbContextLocator ambientDbContextLocator) :
            base(dbContextScopeFactory, ambientDbContextLocator)
        {
        }

        public int GetMinHeight()
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var height = 0;

                try
                {
                    height = _repository.DbContext.Set<RichStat>()
                        .Max(a => a.Height);
                }
                catch (Exception)
                {
                }

                return height;
            }
        }

        public virtual List<RichStat> GetAll(int lastCount, bool includeDeleted = false)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.GetAll(includeDeleted)
                    .OrderByDescending(a => a.Amount)
                    .Take(lastCount)
                    .ToList();
            }
        }

        public void AddWithTransaction(List<RichStat> entities)
        {
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    if (entities != null)
                    {
                        foreach (var item in entities)
                        {
                            _repository.DbContext.Entry(item).State = EntityState.Added;
                        }

                        scope.CommitInternal();
                    }
                }
                catch (Exception)
                {
                    scope.RollbackInternal();
                }
            }
        }

        public void UpdateWithTransaction(List<RichStat> entities)
        {
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    if (entities != null)
                    {
                        foreach (var item in entities)
                        {
                            item.UpdatedUtc = DateTime.UtcNow;
                            _repository.DbContext.Entry(item).State = EntityState.Modified;
                        }

                        scope.CommitInternal();
                    }
                }
                catch (Exception)
                {
                    scope.RollbackInternal();
                }
            }
        }

        public int UpdateHeight(int height)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DbContext.Database
                    .ExecuteSqlCommand("UPDATE RichStats SET Height = " + height);

            }
        }
    }
}
