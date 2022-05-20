using BitcoinRhExplorer.Server;
using BitcoinRhExplorer.Server.Business;

namespace BitcoinRhExplorer.Components
{
    public static class InitHelper
    {
        public static UserComponent Users(UserComponent users)
        {
            if (users == null)
            {
                users = new UserComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);                
            }
            return users;
        }

        public static RoleComponent Roles(RoleComponent roles)
        {
            if (roles == null)
            {
                roles = new RoleComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);                
            }
            return roles;
        }
    }
}