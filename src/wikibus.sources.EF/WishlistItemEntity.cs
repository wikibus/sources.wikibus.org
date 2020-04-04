using System.ComponentModel.DataAnnotations;
using NullGuard;

namespace Wikibus.Sources.EF
{
    [NullGuard(ValidationFlags.None)]
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
