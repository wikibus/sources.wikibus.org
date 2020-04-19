using System.ComponentModel;
using Argolis.Hydra.Annotations;
using Argolis.Hydra.Core;
using JetBrains.Annotations;
using JsonLD.Entities;
using JsonLD.Entities.Context;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vocab;
using Wikibus.Common;
using Wikibus.Common.JsonLd;

namespace Wikibus.Nancy
{
    /// <summary>
    /// The API entry point
    /// </summary>
    [SupportedClass(Api.EntryPoint)]
    public sealed class EntryPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntryPoint"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public EntryPoint(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Id { get; private set; }

        [JsonProperty(Vocab.Hydra.title)]
        public string Title { get; set; }

        [JsonProperty(Vocab.Hydra.description)]
        public string Description { get; set; }

        /// <summary>
        /// Gets the brochures Uri.
        /// </summary>
        [Link]
        [ReadOnly(true)]
        [Range(Vocab.Hydra.Collection)]
        [Description("Brochures and other promotional publications")]
        public IriRef Brochures
        {
            get { return (IriRef)"brochures"; }
        }

        /// <summary>
        /// Gets the books Uri.
        /// </summary>
        [Link]
        [ReadOnly(true)]
        [Range(Vocab.Hydra.Collection)]
        [Description("Books about public transport history and manufacturers, etc.")]
        public IriRef Books
        {
            get { return (IriRef)"books"; }
        }

        /// <summary>
        /// Gets the magazines Uri.
        /// </summary>
        [Link]
        [ReadOnly(true)]
        [Range(Vocab.Hydra.Collection)]
        [Description("Periodicals such as weekly/monthly magazines as well as yearly catalogs")]
        public IriRef Magazines
        {
            get { return (IriRef)"magazines"; }
        }

        /// <summary>
        /// Gets the wishlist Uri.
        /// </summary>
        [Link]
        [ReadOnly(true)]
        [Range(Vocab.Hydra.Collection)]
        [Description("Requests to scan brochures from the collection")]
        public IriRef Wishlist => (IriRef)"wishlist";

        [ReadOnly(true)]
        [Writeable(false)]
        [Range(Schema.CreativeWork)]
        public IriRef[] HasPart
        {
            get
            {
                return new[]
                {
                    (IriRef)"https://cms.wikibus.org/contributing/brochures"
                };
            }
        }

        [UsedImplicitly]
        private static JObject Context
        {
            get
            {
                return new JObject(
                    Prefix.Of(typeof(Wbo)),
                    Prefix.Of(typeof(Api)),
                    "magazines".IsProperty(Api.magazines).Type().Id(),
                    "brochures".IsProperty(Api.brochures).Type().Id(),
                    "books".IsProperty(Api.books).Type().Id(),
                    "wishlist".IsProperty(Api.wishlist).Type().Id(),
                    "hasPart".IsProperty(Schema.hasPart).Type().Id());
            }
        }

        [UsedImplicitly, JsonProperty]
        private string Type
        {
            get { return Api.EntryPoint; }
        }

        [UsedImplicitly]
        private static JObject GetContext(EntryPoint p)
        {
            var context = Context;
            context.AddFirst(Base.Is(p.Id));
            return context;
        }
    }
}
