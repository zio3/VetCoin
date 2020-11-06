using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.MyPage
{
    public class EditModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public EditModel(ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        [BindProperty]
        public VetMember VetMember { get; set; }
        public CoreService CoreService { get; }

        public async Task<IActionResult> OnGetAsync()
        {
            await Task.Yield();
            var userContext = CoreService.GetUserContext();
            VetMember = userContext.CurrentUser;

            if (VetMember == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //DbContext.Attach(VetMember).State = EntityState.Modified;

            var userContext = CoreService.GetUserContext();
            var entity = userContext.CurrentUser;

            await TryUpdateModelAsync(entity, nameof(VetMember));


            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VetMemberExists(VetMember.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool VetMemberExists(int id)
        {
            return DbContext.VetMembers.Any(e => e.Id == id);
        }
    }
}
