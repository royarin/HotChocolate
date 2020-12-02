using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.Stitching;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace graphqlgateway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IIdentityServerClient, IdentityServerClient>(_ =>
            {
                return new IdentityServerClient(_.GetRequiredService<IHttpClientFactory>(), new ClientCredentialsTokenRequest
                {
                    Address = "https://auth.europe-west1.gcp.commercetools.com/oauth/token?grant_type=client_credentials",
                    ClientId = "",
                    ClientSecret = "",
                });
            });

            services.AddTransient<AddTokenHandler>();

            services.AddHttpClient("CommerceToolsGraphQL", client =>
            {
                client.BaseAddress = new Uri("https://api.europe-west1.gcp.commercetools.com/schemastitch/graphql");
            }).AddHttpMessageHandler<AddTokenHandler>();

            services.AddGraphQLServer()
            .AddRemoteSchema("CommerceToolsGraphQL");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // By default the GraphQL server is mapped to /graphql
                // This route also provides you with our GraphQL IDE. In order to configure the
                // the GraphQL IDE use endpoints.MapGraphQL().WithToolOptions(...).
                endpoints.MapGraphQL();
            });

            // app.UseGraphQL();
            app.UsePlayground();
        }
    }
}
