using Argolis.Hydra.Annotations;
using Wikibus.Common;

namespace Wikibus.Sources
{
    [SupportedClass(Wbo.BrochureCollection)]
    public class BrochureCollection : SearchableCollection<Brochure>
    {
        public string[] Types =>
            new[]
            {
                base.Type,
                Wbo.BrochureCollection
            };
    }
}
