﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Argolis.Hydra;
using Argolis.Hydra.Discovery.SupportedClasses;
using Argolis.Hydra.Resources;
using Argolis.Models;
using Argolis.Nancy;
using Nancy;
using Nancy.ModelBinding;
using Wikibus.Sources.Filters;

namespace Wikibus.Sources.Nancy
{
    /// <summary>
    /// Module, which serves <see cref="Source"/>s
    /// </summary>
    public sealed class SourcesModule : ArgolisModule
    {
        private const int PageSize = 12;

        private readonly IUriTemplateExpander expander;
        private readonly IIriTemplateFactory templateFactory;
        private readonly ISupportedClassMetaProvider supportedClass;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourcesModule" /> class.
        /// </summary>
        public SourcesModule(
            ISourcesRepository repository,
            IUriTemplateExpander expander,
            IIriTemplateFactory templateFactory,
            IModelTemplateProvider modelTemplateProvider,
            ISupportedClassMetaProvider supportedClass)
            : base(modelTemplateProvider)
        {
            this.expander = expander;
            this.expander = expander;
            this.templateFactory = templateFactory;
            this.supportedClass = supportedClass;

            this.Get<Brochure>(async r => await this.GetSingle(repository.GetBrochure));
            this.Get<Book>(async r => await this.GetSingle(repository.GetBook));
            this.Get<Magazine>(async r => await this.GetSingle(repository.GetMagazine));
            this.Get<Collection<Issue>>(async r => await this.GetSingle(repository.GetMagazineIssues, new Collection<Issue>()));
            this.Get<Issue>(async r => await this.GetSingle(repository.GetIssue));

            using (this.Templates)
            {
                this.Get<Collection<Brochure>>(async r => await this.GetPage<Brochure, BrochureFilters>((int?)r.page, repository.GetBrochures));
                this.Get<Collection<Magazine>>(async r => await this.GetPage<Magazine, MagazineFilters>((int?)r.page, repository.GetMagazines));
                this.Get<Collection<Book>>(async r => await this.GetPage<Book, BookFilters>((int?)r.page, repository.GetBooks));
            }
        }

        private async Task<dynamic> GetSingle<T>(Func<Uri, Task<T>> getResource, T defaultValue = null)
            where T : class
        {
            Uri resourceUri = this.expander.ExpandAbsolute<T>(this.Context.Parameters);
            var resource = await getResource(resourceUri) ?? defaultValue;

            if (resource == null)
            {
                return 404;
            }

            return this.Negotiate.WithModel(resource).WithHeader("Link", $"<{resourceUri}>; rel=\"canonical\"");
        }

        private async Task<dynamic> GetPage<T, TFilter>(int? page, Func<Uri, TFilter, int, int, Task<SearchableCollection<T>>> getPage)
            where T : class
            where TFilter : ITemplateParameters<Collection<T>>
        {
            if (page == null)
            {
                page = 1;
            }

            if (page < 0)
            {
                return 400;
            }

            var searchTemplate = this.templateFactory.CreateIriTemplate<TFilter, Collection<T>>();
            var uriTemplate = new UriTemplate.Core.UriTemplate(searchTemplate.Template);

            var templateParams = new Dictionary<string, object>((DynamicDictionary)this.Context.Request.Query);
            templateParams.Remove("page");
            var collectionId = uriTemplate.BindByName(templateParams);

            templateParams["page"] = page;
            var canonical = uriTemplate.BindByName(templateParams).ToString();

            var filter = this.Bind<TFilter>();
            var collection = await getPage(collectionId, filter, page.Value, PageSize);
            collection.Title = this.supportedClass.GetMeta(typeof(T)).Description + " collection";

            collection.Views = new IView[]
            {
                new TemplatedPartialCollectionView(uriTemplate, "page", collection.TotalItems, page.Value, PageSize, templateParams),
            };

            collection.Search = searchTemplate;
            collection.CurrentMappings = templateParams.ToTemplateMappings(searchTemplate);

            return this.Negotiate.WithModel(collection).WithHeader("Link", $"<{canonical}>; rel=\"canonical\"");
        }
    }
}
