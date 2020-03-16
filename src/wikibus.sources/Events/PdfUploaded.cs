using System;

namespace Wikibus.Sources.Events
{
    public class PdfUploaded
    {
        public const string Queue = "pdf-uploads";

        public string BlobUri { get; set; }

        public string Name { get; set; }

        public string SourceId { get; set; }
    }
}
