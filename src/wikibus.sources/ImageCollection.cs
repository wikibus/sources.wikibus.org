using System;
using Argolis.Hydra.Core;
using Argolis.Hydra.Resources;
using JsonLD.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vocab;
using Wikibus.Sources.Images;

namespace Wikibus.Sources
{
    public class ImageCollection : Collection<Image>
    {
        public ImageCollection(Uri owner)
        {
            this.Manages.Add(new ManagesBlock((IriRef)owner, (IriRef)Schema.image));
        }

        [JsonProperty("@context")]
        public JToken AtContext => Context;
    }
}
