using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Argolis.Hydra.Annotations;
using Argolis.Hydra.Models;
using Argolis.Models;
using JsonLD.Entities;
using JsonLD.Entities.Context;
using Newtonsoft.Json.Linq;
using NullGuard;
using Vocab;
using Wikibus.Common;

namespace Wikibus.Sources
{
    /// <summary>
    /// A brochure about buses, trams, etc.
    /// </summary>
    [SupportedClass(Wbo.Brochure)]
    [Identifier("brochure/{id}")]
    [CollectionIdentifier("brochures{?page}")]
    [NullGuard(ValidationFlags.ReturnValues)]
    [Description("Public transport brochure")]
    public class Brochure : Source
    {
        private string description;
        private string code;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Required]
        public string Title { [return: AllowNull] get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            [return: AllowNull]
            get
            {
                return this.description;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                this.description = value;
            }
        }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        [ReadOnly(true)]
        public string Code
        {
            [return: AllowNull]
            get
            {
                return this.code;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                this.code = value;
            }
        }

        [Writeable(false)]
        public StorageLocation Location { [return: AllowNull] get; set; }

        [Link]
        [Readable(false)]
        [Writeable(false)]
        public IriRef WishlistItem { [return: AllowNull] get; set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        protected static new JObject Context
        {
            get
            {
                var context = Source.Context;
                context.Add("description".IsProperty(Rdfs.comment));
                context.Add("location".IsProperty(Api.storageLocation));
                context.Add("wishlistItem".IsProperty(Api.wishlistItem));
                return context;
            }
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        protected override IEnumerable<string> Types
        {
            get
            {
                foreach (var type in base.Types)
                {
                    yield return type;
                }

                yield return Type;
            }
        }

        private static string Type => Wbo.Brochure;
    }
}
