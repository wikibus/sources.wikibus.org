using Argolis.Models;

namespace Wikibus.Sources.Functions
{
    internal class BaseUriProvider : IBaseUriProvider
    {
        public string BaseResourceUri => "https://sources.wikibus.org/";
    }
}
