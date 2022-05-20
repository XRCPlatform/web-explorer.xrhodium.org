using EFCache;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinRhExplorer.Server
{
    public class BitcoinRhExplorerDbCacheConfiguration : DbConfiguration
    {
        public static readonly InMemoryCache EntityFrameworkLevel2Cache = new InMemoryCache();

        public BitcoinRhExplorerDbCacheConfiguration()
        {
            var transactionHandler = new CacheTransactionHandler(EntityFrameworkLevel2Cache);

            AddInterceptor(transactionHandler);

            var cachingPolicy = new ServerCoreCachingPolicy();

            Loaded +=
                (sender, e) => e.ReplaceService<DbProviderServices>(
                    (s, _) => new CachingProviderServices(s, transactionHandler, cachingPolicy));
        }
    }

    public class ServerCoreCachingPolicy : CachingPolicy
    {
        protected override bool CanBeCached(System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Entity.Core.Metadata.Edm.EntitySetBase> affectedEntitySets, string sql, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (BitcoinRhExplorerServer.Current.EntityFrameworkLevel2CacheDefinition == BitcoinRhExplorerServer.EntityFrameworkLevel2CacheDefinitionTypes.CacheAllExcludedDefined)
            {
                if (BitcoinRhExplorerServer.Current.EntityFrameworkLevel2CacheExcludedTables != null)
                {
                    foreach (var excludedItem in BitcoinRhExplorerServer.Current.EntityFrameworkLevel2CacheExcludedTables)
                    {
                        var finded = affectedEntitySets.FirstOrDefault(x => x.Name == excludedItem);

                        if (finded != null)
                        {
                            return false;
                            break;
                        }
                    }
                }

                return base.CanBeCached(affectedEntitySets, sql, parameters);
            }
            else
            {
                if (BitcoinRhExplorerServer.Current.EntityFrameworkLevel2CacheExcludedTables != null)
                {
                    foreach (var excludedItem in BitcoinRhExplorerServer.Current.EntityFrameworkLevel2CacheExcludedTables)
                    {
                        var finded = affectedEntitySets.FirstOrDefault(x => x.Name == excludedItem);

                        if (finded != null)
                        {
                            return base.CanBeCached(affectedEntitySets, sql, parameters);
                            break;
                        }
                    }
                }

                return false;
            }
        }
    }
}