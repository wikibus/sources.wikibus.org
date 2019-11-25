using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Argolis.Hydra.Annotations;
using Argolis.Models;
using Newtonsoft.Json;
using NullGuard;
using Vocab;
using Wikibus.Common;

namespace Wikibus.Sources
{
    [NullGuard(ValidationFlags.ReturnValues)]
    [Identifier("storage-location/{brochureId}")]
    [SupportedClass(Api.StorageLocation)]
    public class StorageLocation
    {
        public Uri Id { get; set; }

        [Required]
        [Writeable(false)]
        [DisplayName("Filing cabinet")]
        [JsonProperty(Schema.name)]
        public string Name { get; set; }

        [Required]
        [Readable(false)]
        [JsonProperty(Dcterms.identifier)]
        public int? FilingCabinetId { get; set; }

        [JsonProperty("http://www.linkedmodel.org/schema/dtype#position")]
        public int? Position { get; set; }
    }
}
