using System;
using System.Threading.Tasks;
using Argolis.Models;
using Argolis.Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace Wikibus.Sources.Nancy
{
    public sealed class SourcesUpdateModule : ArgolisModule
    {
        private readonly IUriTemplateExpander expander;

        public SourcesUpdateModule(
            ISourcesPersistence persistence,
            ISourcesRepository repository,
            IModelTemplateProvider modelTemplateProvider,
            IUriTemplateExpander expander)
            : base(modelTemplateProvider)
        {
            this.RequiresAuthentication();

            this.expander = expander;
            this.Put<Brochure>(async r => await this.PutSingle(persistence.SaveBrochure, repository.GetBrochure));
        }

        private async Task<T> PutSingle<T>(
            Func<T, Task> saveResource,
            Func<Uri, Task<T>> getResource)
            where T : Source, new()
        {
            var brochure = this.BindTo(
                new T
                {
                    Id = this.expander.ExpandAbsolute<T>(this.Context.Parameters)
                },
                new BindingConfig { Overwrite = true },
                "Id");
            await saveResource(brochure);

            return await getResource(brochure.Id);
        }
    }
}
