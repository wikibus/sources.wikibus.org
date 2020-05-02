using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NullGuard;
using Vocab;

namespace Wikibus.Sources.Functions.Model
{
    [NullGuard(ValidationFlags.ReturnValues)]
    public class Person
    {
        public Uri Id { get; set; }

        public string Name { get; set; }

        public ImageObject Image { get; set; }

        [JsonProperty]
        private JObject Context => new JObject(
            JsonLD.Entities.Context.Vocab.Is(Schema.BaseUri));

        [JsonProperty]
        private string Type => Schema.Person;
    }
}
