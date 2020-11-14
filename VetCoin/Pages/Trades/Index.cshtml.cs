using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Trades
{
    public class IndexModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }

        public UserContext UserContext { get; set; }

        public IndexModel(VetCoin.Data.ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        public IQueryable<Trade> TradeQuery { get; set; }

        public Direction? Direction { get; set; }
        public CoreService CoreService { get; }

        public void OnGet(string searchKey, Direction? direction)
        {
            Direction = direction;
            UserContext = CoreService.GetUserContext();

            TradeQuery = DbContext.Trades
                .Include(t => t.VetMember)
                .Where(c=>c.TradeStatus != TradeStatus.Cancel)
                .AsQueryable();


            if (direction.HasValue)
            {
                TradeQuery = TradeQuery.Where(c => c.Direction == direction);
            }

            if (!string.IsNullOrEmpty(searchKey))
            {   
                TradeQuery = TradeQuery
                    .Where(c =>
                    c.Title.Contains(searchKey) ||
                    c.Content.Contains(searchKey) ||
                    c.VetMember.Name.Contains(searchKey));
            }
        }
    }
}
