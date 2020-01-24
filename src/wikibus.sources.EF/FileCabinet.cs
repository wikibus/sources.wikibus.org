using System.ComponentModel.DataAnnotations;

namespace Wikibus.Sources.EF
{
    public class FileCabinet
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
