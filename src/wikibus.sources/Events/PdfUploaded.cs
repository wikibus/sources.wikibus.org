using System;

namespace Wikibus.Sources.Events
{
    public class PdfUploaded
    {
        public string BlobUri { get; set; }

        public string Name { get; set; }

        public Uri SourceId { get; set; }
    }
}
