using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities.Users;
using BitcoinRhExplorer.Library;

namespace BitcoinRhExplorer.Server.Business
{
    public class RoleComponent : BaseDbComponent<Role>
    {
        public RoleComponent(IDbContextScopeFactory dbContextScopeFactory, IAmbientDbContextLocator ambientDbContextLocator) :
            base(dbContextScopeFactory, ambientDbContextLocator)
        {
        }

        public Role GetByName(string roleName)
        {
            IEnumerable<Role> role;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                role = _repository.FindBy(r => r.Name == roleName)
                    .ToList();
            }

            if (role.IsAny())
            {
                return role.First();
            }
            else
            {
                throw new Exception("Wrong role");
            }
        }
    }
}
