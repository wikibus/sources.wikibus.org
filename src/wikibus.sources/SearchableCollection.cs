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
        public SearchableCollection()
        {
            this.CurrentMappings = new Dictionary<string, string>();
        }

        public IriTemplate Search { get; set; }

        [JsonProperty("hex:currentMappings")]
        public IDictionary<string, string> CurrentMappings { get; set; }

        [JsonProperty(Rdfs.label)]
        public string Title { get; set; } = "Collection";

        [JsonProperty]
        protected static new JToken Context => Collection<T>.Context.MergeWith(Hex.Context);
    }
}
