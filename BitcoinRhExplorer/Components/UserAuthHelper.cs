using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using BitcoinRhExplorer.Entities.Users;

namespace BitcoinRhExplorer.Components
{
    public static class UserAuthHelper
    {
        internal static void SetupFormAuthTicket(User user, bool tokenVerified = true)
        {
            var json = new JavaScriptSerializer();

            var ticketData = new UserTicket();
            ticketData.UserId = user.Id;
            ticketData.FullName = string.Format("{0} {1}", user.FirstName, user.LastName);
            ticketData.Email = user.Email;
            ticketData.TokenState = 2;

            if (user.IsTwoFactor && !tokenVerified)
            {
                ticketData.TokenState = 1;  
            }

            var authTicket = new FormsAuthenticationTicket(1,
                user.UserName,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                false,
                json.Serialize(ticketData));

            var encTicket = FormsAuthentication.Encrypt(authTicket);

            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }
    }
}