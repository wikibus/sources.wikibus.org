using Argolis.Hydra.Annotations;
using Argolis.Hydra.Resources;
using Argolis.Models;
using NullGuard;
using Vocab;
using Wikibus.Common;

namespace Wikibus.Sources.Filters
{
    /// <summary>
    /// Defines filters of the brochures collection
    /// </summary>
    [NullGuard(ValidationFlags.None)]
    public class BrochureFilters : SourceFilters, ITemplateParameters<Collection<Brochure>>
    {
        [Variable("title")]
        [Property(Dcterms.title)]
        public string Title { get; set; }

        [Variable("withPdfOnly")]
        [Property(Api.withPdfOnly)]
        public bool? WithPdfOnly { get; set; }

        [Variable("withoutImages")]
        [Property(Api.withoutImages)]
        public bool? WithoutImages { get; set; }
    }
}
