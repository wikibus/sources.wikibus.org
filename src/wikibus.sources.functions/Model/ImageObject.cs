using System;
using JsonLD.Entities;
using Newtonsoft.Json;
using Vocab;

namespace Wikibus.Sources.Functions.Model
{
    public class ImageObject
    {
        public IriRef ContentUrl { get; set; }

        [JsonProperty]
        private string Type => Schema.ImageObject;
    }
}
