using System.Collections.Generic;
using System.IO;

namespace wikibus.sources.pdf
{
    public interface IPdfService
    {
        IEnumerable<Stream> ToImages(Stream pdf);
    }
}
