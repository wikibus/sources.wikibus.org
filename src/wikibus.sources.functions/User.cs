using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Anotar.Serilog;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Schema.NET;

namespace Wikibus.Sources.Functions
{
    public class User
    {
        private readonly ManagementClientFactory clientFactory;
        private readonly IConfiguration configuration;

        public User(ManagementClientFactory clientFactory, IConfiguration configuration)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
        }

        [FunctionName("User")]
        public async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "user/{id}")]
            HttpRequest request,
            string id)
        {
            var auth0Client = await this.clientFactory.Create();
            Auth0.ManagementApi.Models.User user;
            try
            {
                user = await auth0Client.Users.GetAsync(id);
            }
            catch (ErrorApiException eae)
            {
                LogTo.Warning(eae, "Failed to load user {0}", id);
                return new NotFoundResult();
            }

            var person = new Person
            {
                Id = new Uri($"{this.configuration["wikibus:usersUrl"]}user/{id}"),
                Name = user.FullName,
                Image = new Values<IImageObject, Uri>(new ImageObject
                {
                    ContentUrl = new Uri(user.Picture)
                })
            };

            return new ContentResult
            {
                Content = person.ToString(),
                ContentType = "application/ld+json"
            };
        }
    }
}
