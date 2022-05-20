using System.Collections.Generic;
using System.Linq;
using BitcoinRhExplorer.Entities.Users;
using BitcoinRhExplorer.Server.Business;
using WebMatrix.WebData;

namespace BitcoinRhExplorer.Components
{
    public class RoleMembershipProvider : SimpleRoleProvider
    {
#pragma warning disable 649
        private UserComponent Users;
        private RoleComponent Roles;
#pragma warning restore 649

        public override bool IsUserInRole(string userName, string roleName)
        {
            var user = InitHelper.Users(Users).GetByUserName(userName);
            var role = InitHelper.Roles(Roles).GetByName(roleName);

            if (user.RoleId == role.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string[] GetRolesForUser(int userId)
        {
            var user = InitHelper.Users(Users).GetById(userId);
            return ReturnRoleName(user);
        }

        public override string[] GetRolesForUser(string userName)
        {
            var user = InitHelper.Users(Users).GetByUserName(userName);
            return ReturnRoleName(user);
        }

        private string[] ReturnRoleName(User user)
        {
            if (user == null)
            {
                return new string[] { UserRole.User.ToString() };
            }

            var role = InitHelper.Roles(Roles).GetById(user.RoleId);
            return new string[] { role.Name };
        }

        public override string[] GetAllRoles()
        {
            IEnumerable<Role> roles = InitHelper.Roles(Roles).GetAll();
            return roles.Select(r => r.Name).ToArray();
        }
    }
}