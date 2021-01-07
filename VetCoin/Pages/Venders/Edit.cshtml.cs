using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Data.VenerEntityes;
using VetCoin.Services;

namespace VetCoin.Pages.Venders
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
        public Vender Vender { get; set; }
        public CoreService CoreService { get; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uc = CoreService.GetUserContext();

            Vender = await DbContext.Venders
                .Include(v => v.VetMember).FirstOrDefaultAsync(m => m.Id == id);

            if (Vender == null)
            {
                return NotFound();
            }

            if (Vender.VetMemberId != uc.CurrentUser.Id)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Vender.HasDeliveryMessage && string.IsNullOrWhiteSpace(Vender.DeliveryMessage))
            {
                ModelState.AddModelError("Vender.DeliveryMessage", "DMメッセージ内容を入力してください");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }


            var entity = DbContext.Venders.Find(Vender.Id);
            await TryUpdateModelAsync(entity, nameof(Vender));

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VenderExists(Vender.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = Vender.Id });
        }

        private bool VenderExists(int id)
        {
            return DbContext.Venders.Any(e => e.Id == id);
        }
    }
}
