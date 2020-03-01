using Anotar.Serilog;
using Nancy.Bootstrapper;

namespace Wikibus.Nancy
{
    public class LogStartup : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.OnError.AddItemToStartOfPipeline((context, exception) =>
            {
                LogTo.Fatal(exception, "Unhandled exception in request");
                return null;
            });
        }
    }
}
