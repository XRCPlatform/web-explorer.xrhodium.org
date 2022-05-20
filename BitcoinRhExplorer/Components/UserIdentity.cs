using System.Security.Principal;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace BitcoinRhExplorer.Components
{
    public class UserTicket
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int TokenState { get; set; }
    }

    public class UserIdentity : IIdentity, IPrincipal
    {
        private readonly FormsAuthenticationTicket _ticket;

        public UserIdentity(FormsAuthenticationTicket ticket)
        {
            _ticket = ticket;
        }

        public string AuthenticationType
        {
            get { return "User"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return _ticket.Name;  }
        }

        public string FullName
        {
            get
            {
                var json = new JavaScriptSerializer();
                var data = json.Deserialize<UserTicket>(_ticket.UserData);

                return data == null ? "N/A (Go to profile)" : (string.IsNullOrWhiteSpace(data.FullName) ? "N/A (Go to profile)" : data.FullName);
            }
        }

        public string Email
        {
            get
            {
                var json = new JavaScriptSerializer();
                var data = json.Deserialize<UserTicket>(_ticket.UserData);

                return data == null ? string.Empty : data.Email;
            }
        }

        public long UserId
        {
            get
            {
                var json = new JavaScriptSerializer();
                var data = json.Deserialize<UserTicket>(_ticket.UserData);

                return data == null ? 0 : data.UserId;
            }
        }

        public int TokenState
        {
            get
            {
                var json = new JavaScriptSerializer();
                var data = json.Deserialize<UserTicket>(_ticket.UserData);

                return data == null ? 0 : data.TokenState;
            }
        }


        public bool IsInRole(string role)
        {
            return Roles.IsUserInRole(role);
        }

        public IIdentity Identity
        {
            get { return this; }
        }
    }
}