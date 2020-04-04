using System.ComponentModel.DataAnnotations;

namespace Wikibus.Sources.EF
{
    public class WishlistItemEntity
    {
        [Key]
        public int Id { get; set; }

        public BrochureEntity Brochure { get; set; }

        public int SourceId { get; set; }

        public string User { get; set; }

        public bool Done { get; set; }
    }
}
