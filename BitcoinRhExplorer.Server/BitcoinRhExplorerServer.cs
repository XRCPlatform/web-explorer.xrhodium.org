using System;
using System.Web;
using System.Web.Hosting;
using System.Configuration;
using BitcoinRhExplorer.EF;
using BitcoinRhExplorer.Library;

namespace BitcoinRhExplorer.Server
{
    public class BitcoinRhExplorerServer : IRegisteredObject
    {
        static BitcoinRhExplorerServer()
        {
            Current = new BitcoinRhExplorerServer();
        }

        public static BitcoinRhExplorerServer Current { get; private set; }
        public EnvironmentTypes Environment { get; set; }

        public DataContextConfiguration DataContextConfiguration { get; private set; }

        public DbContextScopeFactory BitcoinRhExplorerDbContextScopeFactory;
        public AmbientDbContextLocator BitcoinRhExplorerDbContextLocator;

        public BitcoinRhExplorerException Errors;
        public BitcoinRhExplorerCache Cache;

        public string[] EntityFrameworkLevel2CacheExcludedTables;
        public EntityFrameworkLevel2CacheDefinitionTypes EntityFrameworkLevel2CacheDefinition;

        public enum EnvironmentTypes
        {
            Production = 0,
            Test = 1 //initialization of database
        }

        public enum EntityFrameworkLevel2CacheDefinitionTypes
        {
            CacheAllExcludedDefined = 0,
            CacheNothingOnlyDefined = 1
        }

        public void Initialize(string[] level2CacheExcludedTables = null,
                               EntityFrameworkLevel2CacheDefinitionTypes level2CacheDefinition = EntityFrameworkLevel2CacheDefinitionTypes.CacheAllExcludedDefined)
        {
            /* Cache */
            Cache = new BitcoinRhExplorerCache();
            EntityFrameworkLevel2CacheExcludedTables = level2CacheExcludedTables;
            EntityFrameworkLevel2CacheDefinition = level2CacheDefinition;
            /* Environment */
            Environment = InitializeEnvironment();

            /* Database */
            DataContextConfiguration = InitializeConfiguration();
            BitcoinRhExplorerDbContextScopeFactory = new DbContextScopeFactory();
            BitcoinRhExplorerDbContextLocator = new AmbientDbContextLocator();

            if (Environment == EnvironmentTypes.Test) //db init only with test
            {
                using (var context = new BitcoinRhExplorerDbContext())
                {
                    context.Initialize();
                }
            }

            /* Errors */
            Errors = new BitcoinRhExplorerException();
        }

        public void Stop(bool immediate)
        {
            
        }

        public static DataContextConfiguration InitializeConfiguration()
        {
            var settings = ConfigurationManager.AppSettings;

            return new DataContextConfiguration
                {
                    ContextName = settings["BitcoinRhExplorer:DbContextName"],
                    SchemaName = settings["BitcoinRhExplorer:DbSchemaName"]
                };
        }

        public static EnvironmentTypes InitializeEnvironment()
        {
            var settings = ConfigurationManager.AppSettings;

            var environment = EnvironmentTypes.Test;

            Enum.TryParse(settings["BitcoinRhExplorer:ServerEnvironment"], out environment);

            return environment;
        }

        public string GetHost
        {
            get
            {
                if (Environment == EnvironmentTypes.Production)
                {
                    return string.Format(
                        "{0}://{1}",
                        HttpContext.Current.Request.Url.Scheme,
                        HttpContext.Current.Request.Url.Host);
                }
                else
                {
                    return string.Format(
                        "{0}://{1}:{2}",
                        HttpContext.Current.Request.Url.Scheme,
                        HttpContext.Current.Request.Url.Host,
                        HttpContext.Current.Request.Url.Port);
                }
            }
        }
    }
}
