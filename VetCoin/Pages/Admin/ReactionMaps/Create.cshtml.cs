using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCoin.Codes;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Admin.ReactionMaps
{
    public class CreateModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public CreateModel(ApplicationDbContext context, SiteContext siteContext, StaticSettings staticSettings)
        {
            DbContext = context;
            SiteContext = siteContext;
            StaticSettings = staticSettings;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ReactionMap ReactionMap { get; set; }
        public SiteContext SiteContext { get; }
        public StaticSettings StaticSettings { get; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            DbContext.ReactionMaps.Add(ReactionMap);
            await DbContext.SaveChangesAsync();
            SiteContext.ClearReactionMap();

            return RedirectToPage("./Index");
        }
    }
}