using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Trades
{
    public class ImagesModel : PageModel
    {
        public ImagesModel(ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }
        
        private readonly VetCoin.Data.ApplicationDbContext DbContext;
        private readonly CoreService CoreService;

        public Trade Trade { get; set; }
        public IFormFile ImageFile { get; set; }

        public int[] ImageIds { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Trade = await DbContext.Trades
                .Include(t => t.VetMember).FirstOrDefaultAsync(m => m.Id == id);

            var userContext = CoreService.GetUserContext();
            if (Trade.VetMemberId != userContext.CurrentUser.Id)
            {
                return NotFound();
            }

            ImageIds = DbContext.TradeImages
                .AsQueryable()
                .Where(c => c.TradeId == id)
                .Select(c => c.Id)
                .ToArray();

            return Page();
        }

        public async Task<IActionResult> AddImage(int id)
        {

            if (ImageFile != null)
            {
                using (var stream = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(stream);

                    
                    DbContext.TradeImages.Add(new TradeImage
                    {
                        ImageContent = stream.ToArray(),
                        TradeId = id
                    });                    
                }
            }

            await DbContext.SaveChangesAsync();

            return await OnGet(id);
        }

        public async Task<IActionResult> RemoveImage(int id, int imageId)
        {
            var entity = await DbContext.TradeImages.FindAsync(imageId);
            DbContext.TradeImages.Remove(entity);
            await DbContext.SaveChangesAsync();
            return await OnGet(id);
        }

        public async Task<IActionResult> OnPostAsync(int id,int? imageId,string mode)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            switch (mode)
            {
                case "remove":
                    return await RemoveImage(id,imageId.Value);
                case "add":
                    return await AddImage(id);
            }

            return NotFound();

        }


    }
}
