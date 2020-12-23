using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetCoin.Codes;
using VetCoin.Data;
using VetCoin.Data.JsonParamEntites;

namespace VetCoin.Pages.Admin
{
    public class SiteSettingsModel : PageModel
    {
        public SiteSettingsModel(ApplicationDbContext dbContext, SiteContext siteContext)
        {
            DbContext = dbContext;
            SiteContext = siteContext;
        }

        public bool SaveSucceed { get; set; }



        [BindProperty]
        public SiteSetting SiteSetting { get; set; }
        public ApplicationDbContext DbContext { get; }
        public SiteContext SiteContext { get; }

        public void OnGet() {
            SiteSetting = DbContext.GetParam<SiteSetting>();
        }

        public async Task OnPostAsync()
        {
            var current = DbContext.GetParam<SiteSetting>();

            DbContext.SetParam(SiteSetting);

            if(current.TitleDescription != SiteSetting.TitleDescription)
            {
                DbContext.RuleTextLogs.Add(new RuleTextLog
                {
                    RuleMarkdown = SiteSetting.TitleDescription
                });
            }

            await DbContext.SaveChangesAsync();
            SaveSucceed = true;
            SiteContext.UpdateSiteSetting(SiteSetting);
        }
    }
}
