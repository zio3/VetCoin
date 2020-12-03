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

        [BindProperty(SupportsGet = true)]
        public int? UnderRewordLimit { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? OverRewordLimit { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsExceptRewardNull { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public bool IsWorking { get; set; } = true;
        
        [BindProperty(SupportsGet = true)]
        public bool IsCpmplited { get; set; } = true;

        public void OnGet(string searchKey, Direction? direction)
        {
            Direction = direction;
            UserContext = CoreService.GetUserContext();

            TradeQuery = DbContext.Trades
                .Include(t => t.VetMember)
                .Where(c=>c.TradeStatus != TradeStatus.Cancel)
                .AsQueryable();

            if(UnderRewordLimit.HasValue)
            {
                TradeQuery = TradeQuery
                    .Where(c => c.Reward >= UnderRewordLimit || c.Reward == null);
            }

            if (OverRewordLimit.HasValue)
            {
                TradeQuery = TradeQuery
                    .Where(c => c.Reward <= OverRewordLimit || c.Reward == null);
            }

            if(IsExceptRewardNull)
            {
                TradeQuery = TradeQuery
                    .Where(c => c.Reward != null);
            }

            if (!IsWorking)
            {
                TradeQuery = TradeQuery.Where(c => !c.Contracts.Any(d => d.ContractStatus == ContractStatus.Working || d.ContractStatus == ContractStatus.Deliveryed ));
            }
            if (!IsCpmplited)
            {
                TradeQuery = TradeQuery.Where(c => !c.Contracts.Any(d => d.ContractStatus == ContractStatus.Complete) || c.IsContinued);
            }

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

            Response.Cookies.Append("Test", "{ a:'BBB'}");
        }
    }
}
