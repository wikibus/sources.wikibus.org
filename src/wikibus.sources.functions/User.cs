using System;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Argolis.Hydra.Resources;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using JsonLD.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wikibus.Sources.EF;
using Wikibus.Sources.Functions.Model;

namespace Wikibus.Sources.Functions
{
    public class User
    {
        private readonly ManagementClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private readonly EntitySerializer entitySerializer = new EntitySerializer();
        private readonly ISourceContext context;

        public User(ManagementClientFactory clientFactory, IConfiguration configuration, ISourceContext context)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.context = context;
        }

        private string ContributorRoleId => this.configuration["authentication:Auth0:Management:Roles:Contributor"];

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
                Identifier = id,
                Name = user.FullName,
                Image = new ImageObject
                {
                    ContentUrl = (IriRef)new Uri(user.Picture)
                }
            };

            return new ContentResult
            {
                Content = this.entitySerializer.Serialize(person).ToString(),
                ContentType = "application/ld+json"
            };
        }

        [FunctionName("Contributors")]
        public async Task<IActionResult> GetAllUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "contributors")]
            HttpRequest request)
        {
            var auth0Client = await this.clientFactory.Create();
            var userIds = await this.context.Brochures.Select(brochure => brochure.User).Distinct().ToListAsync();
            var userIdQuery = string.Join(" or ", userIds.Select(id => $"user_id:\"{id}\""));

            var users = await auth0Client.Users.GetAllAsync(
                new GetUsersRequest
                {
                    Query = userIdQuery
                },
                new PaginationInfo(includeTotals: true));

            var members = users.Select(user => new Person
            {
                Id = new Uri($"{this.configuration["wikibus:usersUrl"]}user/{user.UserId}"),
                Name = user.FullName,
                Identifier = user.UserId,
            }).ToArray();

            var collection = new Collection<Person>
            {
                Id = new Uri($"{this.configuration["wikibus:usersUrl"]}contributors"),
                Members = members,
                TotalItems = users.Paging.Total,
            };

            return new ContentResult
            {
                Content = this.entitySerializer.Serialize(collection).ToString(),
                ContentType = "application/ld+json"
            };
        }
    }
}
