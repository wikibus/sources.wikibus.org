using System;
using Argolis.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NullGuard;
using Vocab;

namespace Wikibus.Sources
{
    [Identifier("{type}/{id}/file")]
    public class SourceContent
    {
        public SourceContent(Uri id, string name, string encodingFormat, int contentSize, [AllowNull] Uri contentUrl)
        {
            this.Id = id;
            this.Name = name;
            this.EncodingFormat = encodingFormat;
            this.ContentUrl = contentUrl;
            this.ContentSizeMb = contentSize;
        }

        [UsedImplicitly]
        public static JObject Context => new JObject(JsonLD.Entities.Context.Vocab.Is(Schema.BaseUri));

        [UsedImplicitly]
        public Uri Id { get; }

        [UsedImplicitly]
        public string Name { get; }

        [UsedImplicitly]
        public string EncodingFormat { get; }

        [UsedImplicitly]
        public string ContentSize => $"{(double)this.ContentSizeMb / 1024 / 1024:F2} MB";

        [UsedImplicitly]
        public Uri ContentUrl { [return: AllowNull] get; }

        [UsedImplicitly]
        public string Type => Schema.MediaObject;

        [JsonIgnore]
        public int ContentSizeMb { get; }
    }
}
