using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace graphqlgateway
{
    public class AddTokenHandler : DelegatingHandler
    {
        private IIdentityServerClient identityServerClient;
        public AddTokenHandler(IIdentityServerClient identityServerClient)
        {
            this.identityServerClient = identityServerClient;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        {
            var token = await identityServerClient.RequestClientCredentialsTokenAsync();
            if (!request.Headers.Contains("Authorization"))
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
