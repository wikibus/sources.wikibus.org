using System.Collections.Generic;
using System.Linq;
using Argolis.Hydra.Core;

namespace Wikibus.Sources.Nancy
{
    public static class CollectionExtensions
    {
        public static IDictionary<string, string> ToTemplateMappings(
            this Dictionary<string, object> templateParams,
            IriTemplate searchTemplate)
        {
            return templateParams.ToDictionary(
                k => searchTemplate.Mappings.FirstOrDefault(mapping => mapping.Variable == k.Key)?.Property.Id ?? k.Key,
                v => v.Value.ToString());
        }
    }
}
