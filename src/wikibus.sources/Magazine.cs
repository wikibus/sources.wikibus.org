﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Argolis.Hydra.Annotations;
using Argolis.Hydra.Models;
using Argolis.Models;
using JetBrains.Annotations;
using JsonLD.Entities.Context;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NullGuard;
using Vocab;
using Wikibus.Common;

namespace Wikibus.Sources
{
    /// <summary>
    /// A periodical about public transport
    /// </summary>
    [NullGuard(ValidationFlags.AllPublic ^ ValidationFlags.Properties)]
    [SupportedClass(Wbo.Magazine)]
    [Identifier("magazine/{name}")]
    [CollectionIdentifier("magazines{?page}")]
    public class Magazine
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Uri Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [ReadOnly(true)]
        public string Title { get; set; }

        /// <summary>
        /// Gets the issues Uri.
        /// </summary>
        [Link]
        [ReadOnly(true)]
        [Range(Hydra.Collection)]
        public Uri Issues
        {
            get { return new Uri(this.Id + "/issues"); }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        [UsedImplicitly]
        internal static JObject Context
        {
            get
            {
                return new JObject(
                    "title".IsProperty(Dcterms.title),
                    "issues".IsProperty(Api.issues).Type().Id());
            }
        }

        private static string Type => Wbo.Magazine;

        [JsonProperty, UsedImplicitly]
        private IEnumerable<string> Types
        {
            get
            {
                yield return Type;
                yield return Schema.Periodical;
            }
        }
    }
}
