using System.Web.Security;
using BitcoinRhExplorer.Server.Business;
using WebMatrix.WebData;

namespace BitcoinRhExplorer.Components
{
    public class MembershipProvider : SimpleMembershipProvider
    {
#pragma warning disable 649
        private readonly UserComponent Users;
#pragma warning restore 649

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return base.GetUser(username, userIsOnline);
        }

        public override bool ValidateUser(string username, string password)
        {
            try
            {
                if (InitHelper.Users(Users).LoginUserByEmail(username, password) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}