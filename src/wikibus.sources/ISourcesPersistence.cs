using System.Threading.Tasks;

namespace Wikibus.Sources
{
    public interface ISourcesPersistence
    {
        Task SaveBrochure(Brochure brochure, bool updateFileContents = false);

        Task CreateBrochure(Brochure brochure, string user);
    }
}
