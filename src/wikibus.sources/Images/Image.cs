using JetBrains.Annotations;
using Newtonsoft.Json;
using NullGuard;
using Vocab;

namespace Wikibus.Sources.Images
{
    /// <summary>
    /// Represents a schema.org ImageObject
    /// </summary>
    [NullGuard(ValidationFlags.AllPublic ^ ValidationFlags.Properties)]
    public abstract class Image
    {
        private int? orderIndex;

        /// <summary>
        /// Gets or sets the content URL.
        /// </summary>
        public string ContentUrl { get; set; }

        [UsedImplicitly]
        public virtual Image Thumbnail { get; private set; }

        [JsonProperty("http://www.linkedmodel.org/schema/dtype#orderIndex")]
        public int? OrderIndex
        {
            [return: AllowNull]
            get => this.IsThumbnail ? null : this.orderIndex;
            set => this.orderIndex = value;
        }

        protected bool IsThumbnail { get; set; }

        [UsedImplicitly]
        private static string Type => Schema.ImageObject;

        /// <summary>
        /// Determines whether image should be serialized
        /// </summary>
        public bool ShouldSerializeThumbnail()
        {
            return this.IsThumbnail == false;
        }
    }
}
