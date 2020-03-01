using System.IO;
using Argolis.Hydra.Annotations;
using Newtonsoft.Json;
using Vocab;
using Wikibus.Common;

namespace Wikibus.Nancy.Requests
{
    [SupportedClass(Api.PdfUpload)]
    public class PdfUpload
    {
        [Range(Schema.MediaObject)]
        [JsonProperty(Schema.associatedMedia)]
        public Stream Contents { get; set; }
    }
}
