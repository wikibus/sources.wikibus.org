using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Argolis.Models;

namespace Wikibus.Sources.EF
{
    public class SourcesPersistence : ISourcesPersistence
    {
        private readonly SourceContext context;
        private readonly IUriTemplateMatcher matcher;
        private readonly IUriTemplateExpander expander;

        public SourcesPersistence(SourceContext context, IUriTemplateMatcher matcher, IUriTemplateExpander expander)
        {
            this.context = context;
            this.matcher = matcher;
            this.expander = expander;
        }

        public async Task SaveBrochure(Brochure brochure)
        {
            var id = this.matcher.Match<Brochure>(brochure.Id).Get<int>("id");
            var brochureEntity = this.context.Brochures.Find(id);

            UpdateEntity(brochure, brochureEntity);

            await this.context.SaveChangesAsync();
        }

        public async Task CreateBrochure(Brochure brochure)
        {
            var brochureEntity = new BrochureEntity
            {
                Image = new ImageData()
            };
            UpdateEntity(brochure, brochureEntity);

            await this.context.Brochures.AddAsync(brochureEntity);
            await this.context.SaveChangesAsync();

            brochure.Id = this.expander.ExpandAbsolute<Brochure>(new { id = brochureEntity.Id });
        }

        private static void UpdateEntity(Brochure brochure, BrochureEntity brochureEntity)
        {
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

            brochureEntity.Pages = brochure.Pages;

            var validLanguages = brochure.Languages.Where(lang => lang != null).Where(lang => lang.IsValid).Select(lang => lang.Name);
            brochureEntity.Languages = string.Join(";", validLanguages);

            brochureEntity.ContentUrl = brochure.Content.ContentUrl?.ToString();
            brochureEntity.ContentSize = brochure.Content.ContentSizeMb;
        }
    }
}
