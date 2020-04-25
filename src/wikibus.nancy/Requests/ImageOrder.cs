using Argolis.Hydra.Annotations;
using Newtonsoft.Json;
using Wikibus.Common;

namespace Wikibus.Nancy.Requests
{
    [SupportedClass(Api.ImageOrder)]
    public class ImageOrder
    {
        [JsonProperty("http://www.linkedmodel.org/schema/dtype#orderIndex")]
        public int OrderIndex { get; set; }
    }
}
