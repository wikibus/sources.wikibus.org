using System.ComponentModel.DataAnnotations;
using NullGuard;

namespace Wikibus.Sources.EF
{
    public class ImageData
    {
        [Key]
        public int Id { get; set; }

        public byte[] Bytes { [return: AllowNull] get; set; }
    }
}
