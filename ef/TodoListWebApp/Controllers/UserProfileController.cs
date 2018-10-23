using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TodoListWebApp.DAL;
//using TodoListWebApp.Models;


namespace TodoListWebApp.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private TodoListWebAppContext db = new TodoListWebAppContext();

        // GET: UserProfile
        public async Task<ActionResult> Index()
        {
                     

            try
            {
                //ActiveDirectoryClient graphClient = new ActiveDirectoryClient(
                //    new Uri(graphResourceID + '/' + tenantID),
                //    () => getTokenForGraph(tenantID, signedInUserID, userObjectID, clientId, appKey, graphResourceID));

                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();


                Microsoft.Graph.User me = await graphClient.Me.Request().GetAsync();
                    
                return View(me);
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (Exception ee)
            {
                ViewBag.ErrorMessage = "AuthorizationRequired";
                return View();
            }
        }
        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/UserProfile" }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        private async Task<string> getTokenForGraph(string tenantID, string signedInUserID, string userObjectID, string clientId, string appKey, string graphResourceID)
        {
            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientcred = new ClientCredential(clientId, appKey);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's EF DB
            AuthenticationContext authContext = new AuthenticationContext(string.Format("https://login.partner.microsoftonline.cn/{0}", tenantID), new EFADALTokenCache(signedInUserID));
            AuthenticationResult result = await authContext.AcquireTokenSilentAsync(graphResourceID, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return result.AccessToken;
        }
    }
}