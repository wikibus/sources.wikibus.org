using System.ComponentModel.DataAnnotations;

namespace Wikibus.Sources.EF
{
    public class ImageEntity
    {
        [Key]
        public int Id { get; set; }

        public string OriginalUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ExternalId { get; set; }

        public short OrderIndex { get; set; }

        public int SourceId { get; set; }
    }
}
