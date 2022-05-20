using CacheManager.Core;

namespace BitcoinRhExplorer.Server
{
    public class BitcoinRhExplorerCache
    {
        public ICacheManager<string> CommonCache { get; private set; }
        public ICacheManager<string> LongCache { get; private set; }

        public BitcoinRhExplorerCache()
        {
            CommonCache = CacheFactory.FromConfiguration<string>("CommonCache");
            LongCache = CacheFactory.FromConfiguration<string>("LongCache");
        }
    }
}
