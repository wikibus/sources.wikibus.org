using System;
using System.Collections.Generic;
using System.Security.Claims;
using Argolis.Hydra.Annotations;
using Argolis.Hydra.Resources;
using JetBrains.Annotations;
using JsonLD.Entities;
using JsonLD.Entities.Context;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NullGuard;
using Vocab;
using Wikibus.Common;
using Wikibus.Common.JsonLd;
using Wikibus.Sources.Images;

namespace Wikibus.Sources
{
    /// <summary>
    /// A bibliographical source of knowledge about public transport
    /// </summary>
    [SupportedClass(Wbo.Source)]
    [NullGuard(ValidationFlags.ReturnValues)]
    public class Source
    {
        private Language[] languages = new Language[0];
        private Uri id;
        private Uri content;
        private int? contentSize;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Uri Id
        {
            [return: AllowNull] get => this.id;
            set
            {
                if (!value.IsAbsoluteUri)
                {
                    return;
                }

                this.Images = new ImageCollection(value);
                this.id = value;
            }
        }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        [Range(Dcterms.language)]
        public Language[] Languages
        {
            get { return this.languages; }
            set { this.languages = value; }
        }

        /// <summary>
        /// Gets or sets the pages count.
        /// </summary>
        [Range(Xsd.integer)]
        public int? Pages { [return: AllowNull] get; set; }

        /// <summary>
        /// Gets or sets the publication date date.
        /// </summary>
        [Range(Xsd.date)]
        public DateTime? Date { [return: AllowNull] get; set; }

        /// <summary>
        /// Gets or sets the publication year.
        /// </summary>
        [Range(Xsd.gYear)]
        public short? Year { [return: AllowNull] get; set; }

        /// <summary>
        /// Gets or sets the publication month.
        /// </summary>
        [Range(Xsd.gMonth)]
        public byte? Month { [return: AllowNull] get; set; }

        /// <summary>
        /// Gets the image.
        /// </summary>
        [Readable(false)]
        [Writeable(false)]
        public Collection<Image> Images { get; private set; }

        [Readable(false)]
        [Writeable(false)]
        public IriRef CoverImage { get; set; }

        [Range(Schema.ImageObject)]
        [Readable(false)]
        [Writeable(false)]
        public Image[] Image { get; set; }

        [Range(Schema.MediaObject)]
        [Writeable(false)]
        public SourceContent Content => new SourceContent(
            new Uri($"{this.Id}/file"),
            "Download PDF",
            MimeMapping.KnownMimeTypes.Pdf,
            this.contentSize ?? 0,
            this.content);

        [Range(Schema.Person)]
        [Writeable(false)]
        public IriRef Contributor => (IriRef)$"https://users.wikibus.org/user/{this.User}";

        [JsonIgnore]
        public string User { get; set; }

        /// <summary>
        /// Gets the @context.
        /// </summary>
        [UsedImplicitly]
        protected static JObject Context
        {
            get
            {
                return new JObject(
                    Prefix.Of(typeof(Bibo)),
                    Prefix.Of(typeof(Dcterms)),
                    Prefix.Of(typeof(Xsd)),
                    Prefix.Of(typeof(Opus)),
                    Prefix.Of(typeof(Rdfs)),
                    Prefix.Of(typeof(Schema)),
                    Prefix.Of(typeof(Wbo)),
                    "langIso".IsPrefixOf(Lexvo.iso639_1),
                    "year".IsProperty(Opus.year).Type().Is(Xsd.gYear),
                    "month".IsProperty(Opus.month).Type().Is(Xsd.gMonth),
                    "date".IsProperty(Dcterms.date).Type().Is(Xsd.date),
                    "pages".IsProperty(Bibo.pages).Type().Is(Xsd.integer),
                    "title".IsProperty(Dcterms.title),
                    "code".IsProperty(Dcterms.identifier),
                    "languages".IsProperty(Dcterms.language).Type().Id().Container().Set(),
                    "name".IsProperty(Schema.name),
                    "coverImage".IsProperty(Schema.primaryImageOfPage),
                    "image".IsProperty(Schema.image),
                    "images".IsProperty(Api.images),
                    "hasImage".IsProperty(Wbo.BaseUri + "hasImage"),
                    "contentUrl".IsProperty(Schema.contentUrl).Type().Is(Schema.URL),
                    "thumbnail".IsProperty(Schema.thumbnail),
                    "content".IsProperty(Schema.associatedMedia),
                    "encodingFormat".IsProperty(Schema.encodingFormat),
                    "contentSize".IsProperty(Schema.contentSize),
                    "contributor".IsPrefixOf(Schema.contributor));
            }
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        [JsonProperty]
        protected virtual IEnumerable<string> Types
        {
            get { yield return Wbo.Source; }
        }

        public void SetContent(Uri uri, int size)
        {
            if (this.content != null)
            {
                throw new InvalidOperationException("Cannot overwrite a file");
            }

            this.content = uri;
            this.contentSize = size;
        }

        public bool OwnedBy(ClaimsPrincipal user)
        {
            return user.GetNameClaim() == this.User;
        }

        public void RemoveContent()
        {
            this.content = null;
            this.contentSize = null;
        }
    }
}
