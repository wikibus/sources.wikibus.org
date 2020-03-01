using System.Collections.Generic;
using System.IO;
using System.Linq;
using NReco.PdfRenderer;

namespace wikibus.sources.pdf
{
    public class NRecoPdfService : IPdfService
    {
        private readonly PdfToImageConverter converter = new PdfToImageConverter();

        public IEnumerable<Stream> ToImages(Stream pdf)
        {
            var imageFiles = this.converter.GenerateImages(pdf, ImageFormat.Jpeg, Path.GetTempPath());

            return imageFiles.Select(File.OpenRead);
        }
    }
}
