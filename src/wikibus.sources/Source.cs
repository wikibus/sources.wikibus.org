using System;
using System.Collections.Generic;
using System.ComponentModel;
using Argolis.Hydra.Annotations;
using JetBrains.Annotations;
using JsonLD.Entities.Context;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NullGuard;
using Vocab;
using Wikibus.Common;
using Wikibus.Common.JsonLd;

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

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Uri Id { [return: AllowNull] get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        [ReadOnly(true)]
        [Range(Xsd.gMonth)]
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
        [ReadOnly(true)]
        [Range(Schema.ImageObject)]
        public Image Image { [return: AllowNull] get; set; }

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
                    "image".IsProperty(Schema.image),
                    "hasImage".IsProperty(Wbo.BaseUri + "hasImage"),
                    "contentUrl".IsProperty(Schema.contentUrl).Type().Is(Schema.URL),
                    "thumbnail".IsProperty(Schema.thumbnail));
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
    }
}
