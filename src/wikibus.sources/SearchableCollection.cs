using System.Collections.Generic;
using Argolis.Hydra.Core;
using Argolis.Hydra.Resources;
using JsonLD.Entities;
using JsonLD.Entities.Context;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vocab;

namespace Wikibus.Sources
{
    [SerializeCompacted]
    public class SearchableCollection<T> : Collection<T>
    {
        public IriTemplate Search { get; set; }

        [JsonProperty("hex:currentMappings")]
        public Dictionary<string, string> CurrentMappings { get; set; }

        [JsonProperty(Rdfs.label)]
        public string Title { get; set; } = "Collection";

        [JsonProperty]
        private static JToken Context => Collection<T>.Context.MergeWith(Hex.Context);
    }
}
