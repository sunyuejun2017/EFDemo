using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace TodoListWebApp.DAL
{
    

    public class SDKHelper
    {
        private static GraphServiceClient graphClient = null;

        // Get an authenticated Microsoft Graph Service client.
        public static GraphServiceClient GetAuthenticatedClient()
        {
            graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        string accessToken = await SampleAuthProvider.Instance.GetUserAccessTokenAsync();

                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                        // Add this header hto identify the sample in the Microsoft Graph service.
                        // requestMessage.Headers.Add("SampleID", "AppName");
                    }));
            graphClient.BaseUrl = "https://microsoftgraph.chinacloudapi.cn" + "/v1.0";
            return graphClient;
        }

        public static void SignOutClient()
        {
            graphClient = null;
        }
    }
}