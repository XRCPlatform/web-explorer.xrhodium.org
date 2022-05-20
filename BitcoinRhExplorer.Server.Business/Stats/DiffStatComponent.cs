using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities.Stats;
namespace BitcoinRhExplorer.Server.Business.Stats
{
    public class DiffStatComponent : BaseDbComponent<DiffStat>
    {
        public DiffStatComponent(IDbContextScopeFactory dbContextScopeFactory, IAmbientDbContextLocator ambientDbContextLocator) :
            base(dbContextScopeFactory, ambientDbContextLocator)
        {
        }
    }
}
