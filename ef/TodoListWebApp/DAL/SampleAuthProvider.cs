using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace TodoListWebApp.DAL
{
    public interface IAuthProvider
    {
        Task<string> GetUserAccessTokenAsync();
    }
    public sealed class SampleAuthProvider : IAuthProvider
    {

      

        private SampleAuthProvider() { }

        public static SampleAuthProvider Instance { get; } = new SampleAuthProvider();


        private async Task<string> getTokenForGraph(string tenantID, string signedInUserID, string userObjectID, string clientId, string appKey, string graphResourceID)
        {
            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientcred = new ClientCredential(clientId, appKey);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's EF DB
            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(string.Format("https://login.partner.microsoftonline.cn/{0}", tenantID), new EFADALTokenCache(signedInUserID));
            AuthenticationResult result = await authContext.AcquireTokenSilentAsync(graphResourceID, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return result.AccessToken;
        }

        // Get an access token. First tries to get the token from the token cache.
        public async Task<string> GetUserAccessTokenAsync()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            HttpContextBase httpContextBase = HttpContext.Current.GetOwinContext().Environment["System.Web.HttpContextBase"] as HttpContextBase;

            EFADALTokenCache tokenCache = new EFADALTokenCache(signedInUserID);
            //var cachedItems = tokenCache.ReadItems(); // see what's in the cache


            string tenantId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
           
            string clientId = ConfigurationManager.AppSettings["ida:ClientID"];
            string appKey = ConfigurationManager.AppSettings["ida:Password"];
            string graphResourceID = "https://microsoftgraph.chinacloudapi.cn";
           
            ClientCredential clientCredential = new ClientCredential(clientId,appKey);

            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(string.Format("https://login.partner.microsoftonline.cn/{0}", tenantId), tokenCache);

            string userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            UserIdentifier userId = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);

            try
            {
                AuthenticationResult result = await authContext.AcquireTokenSilentAsync(graphResourceID, clientCredential, userId);

                return result.AccessToken;
            }
            // Unable to retrieve the access token silently.
            catch (AdalException ex)
            {
                HttpContext.Current.Request.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties() { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);

                throw new Exception($" {ex.Message}");
            }
        }
    }
}