using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

namespace BitcoinRhExplorer.App_Start
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            WebSecurity.InitializeDatabaseConnection("BitcoinRhExplorerDbContext", "Users", "Id", "UserName",
                                                                 autoCreateTables: false);

            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166
        }
    }
}
