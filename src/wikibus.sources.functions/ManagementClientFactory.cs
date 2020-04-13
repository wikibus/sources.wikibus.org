using System;
using System.Threading.Tasks;
using Anotar.Serilog;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Microsoft.Extensions.Configuration;

namespace Wikibus.Sources.Functions
{
    public class ManagementClientFactory
    {
        private readonly AuthenticationApiClient authClient;
        private readonly IConfiguration configuration;
        private AccessTokenResponse token;
        private DateTime tokenExpiration;

        public ManagementClientFactory(IConfiguration configuration)
        {
            this.configuration = configuration;

            this.authClient = new AuthenticationApiClient(this.Domain);
        }

        private string Domain => this.configuration["authentication:Auth0:Domain"];

        private string ClientId => this.configuration["authentication:Auth0:ClientId"];

        private string ClientSecret => this.configuration["authentication:Auth0:ClientSecret"];

        private AccessTokenResponse Token
        {
            get => this.token;
            set
            {
                this.token = value;
                this.tokenExpiration = DateTime.Now.AddSeconds(this.token.ExpiresIn);
            }
        }

        private bool TokenExpired => this.tokenExpiration < DateTime.Now;

        public async Task<ManagementApiClient> Create()
        {
            if (this.token == null)
            {
                LogTo.Information("Getting management API auth token");
                this.Token = await this.authClient.GetTokenAsync(new ClientCredentialsTokenRequest
                {
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    Audience = "https://wikibus.eu.auth0.com/api/v2/"
                });
            }

            if (this.TokenExpired)
            {
                this.Token = await this.authClient.GetTokenAsync(new RefreshTokenRequest
                {
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    RefreshToken = this.Token.RefreshToken,
                    Audience = "https://wikibus.eu.auth0.com/api/v2/"
                });
            }

            return new ManagementApiClient(this.Token.AccessToken, this.Domain);
        }
    }
}
