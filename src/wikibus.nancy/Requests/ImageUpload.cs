using Argolis.Hydra.Annotations;
using Newtonsoft.Json;
using Vocab;
using Wikibus.Common;
using Wikibus.Sources.Images;

namespace Wikibus.Nancy.Requests
{
    [SupportedClass(Api.ImageUpload)]
    public class ImageUpload
    {
        [Range(Schema.ImageObject)]
        [JsonProperty(Schema.image)]
        public Image[] Files { get; set; }
    }
}
