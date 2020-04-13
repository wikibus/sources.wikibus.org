using System;
using System.Threading.Tasks;
using Anotar.Serilog;
using Auth0.Core.Exceptions;
using JsonLD.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Wikibus.Sources.Functions.Model;

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
                Image = new ImageObject
                {
                    ContentUrl = (IriRef)new Uri(user.Picture)
                }
            };

            return new ContentResult
            {
                Content = new EntitySerializer().Serialize(person).ToString(),
                ContentType = "application/ld+json"
            };
        }
    }
}
