using System.Threading.Tasks;
using Argolis.Models;

namespace Wikibus.Sources.EF
{
    public class SourcesPersistence : ISourcesPersistence
    {
        private readonly SourceContext context;
        private readonly IUriTemplateMatcher matcher;

        public SourcesPersistence(SourceContext context, IUriTemplateMatcher matcher)
        {
            this.context = context;
            this.matcher = matcher;
        }

        public async Task SaveBrochure(Brochure brochure)
        {
            var id = this.matcher.Match<Brochure>(brochure.Id).Get<int>("id");
            var brochureEntity = this.context.Brochures.Find(id);

            brochureEntity.FolderName = brochure.Title;
            brochureEntity.Notes = brochure.Description;
            if (brochure.Date.HasValue)
            {
                var date = brochure.Date.Value;
                brochureEntity.Day = (byte)date.Day;
                brochureEntity.Month = (byte)date.Month;
                brochureEntity.Year = (byte)date.Year;
            }
            else
            {
                brochureEntity.Day = null;
                brochureEntity.Year = brochure.Year;
                brochureEntity.Month = brochure.Month;
            }

            /*brochureEntity.Language = null;
            brochureEntity.Language2 = null;
            for (var i = 0; i < brochure.Languages.Take(2).Count(); i++)
            {
                var language = brochure.Languages[i];
                if (i == 0)
                {
                    brochureEntity.Language = language.Name;
                }
                else if (i == 1)
                {
                    brochureEntity.Language2 = language.Name;
                }
            }*/

            brochureEntity.Pages = brochure.Pages;

            await this.context.SaveChangesAsync();
        }
    }
}
