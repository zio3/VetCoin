using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;

namespace VetCoin.Pages.Trades
{
    public class IndexModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }

        public IndexModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<Trade> TradeQuery { get; set; }

        public Direction? Direction { get; set; }

        public void OnGet(string searchKey, Direction? direction)
        {
            Direction = direction;

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
