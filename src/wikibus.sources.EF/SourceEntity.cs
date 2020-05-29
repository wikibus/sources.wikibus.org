using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using NullGuard;

namespace Wikibus.Sources.EF
{
    [NullGuard(ValidationFlags.None)]
    [Table("Source", Schema = "Sources")]
    public class SourceEntity : IHasImage
    {
        [Key]
        public int Id { get; set; }

        public string Languages { get; set; }

        public int? Pages { get; set; }

        public short? Year { get; set; }

        public byte? Month { get; set; }

        public byte? Day { get; set; }

        [CanBeNull] public string ContentUrl { get; set; }

        public int? ContentSize { get; set; }

        public virtual ImageData Image { get; set; }

        public string User { get; set; }

        [UsedImplicitly]
        public byte[] Updated { get; set; }

        public IList<ImageEntity> Images { get; set; } = new List<ImageEntity>();
    }
}
