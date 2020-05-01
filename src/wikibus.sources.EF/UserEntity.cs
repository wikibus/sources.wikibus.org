using System.ComponentModel.DataAnnotations;

namespace Wikibus.Sources.EF
{
    public class UserEntity
    {
        [Key]
        public string UserId { get; set; }

        public string DriveImportFolder { get; set; }
    }
}
