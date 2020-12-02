using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace graphqlgateway
{
    public interface IIdentityServerClient
    {
        Task<string> RequestClientCredentialsTokenAsync();
    }

    public class IdentityServerClient : IIdentityServerClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ClientCredentialsTokenRequest tokenRequest;

        public IdentityServerClient(
            IHttpClientFactory httpClient,
            ClientCredentialsTokenRequest tokenRequest)
        {
            this.httpClientFactory = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.tokenRequest = tokenRequest ?? throw new ArgumentNullException(nameof(tokenRequest));
        }

        public async Task<string> RequestClientCredentialsTokenAsync()
        {
            // request the access token token
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenRequest.Address);
            var authenticationBytes = Encoding.ASCII.GetBytes($"{tokenRequest.ClientId}:{tokenRequest.ClientSecret}");

            var authHeaderValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authenticationBytes));
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = authHeaderValue;

            var responseMessage = await httpClient.SendAsync(requestMessage);

            if (responseMessage.IsSuccessStatusCode == false)
            {
                //TODO: exception handling
                throw new Exception("Could not get Commerce Tools access token");
            }

            string token = JsonConvert.DeserializeObject<dynamic>(await responseMessage.Content.ReadAsStringAsync()).access_token;
            return token;
        }
    }

}