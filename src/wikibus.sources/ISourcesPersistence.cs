using System.Threading.Tasks;

namespace Wikibus.Sources
{
    public interface ISourcesPersistence
    {
        Task SaveBrochure(Brochure brochure);
    }
}
