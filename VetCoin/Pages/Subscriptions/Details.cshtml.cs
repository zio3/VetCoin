using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Subscriptions
{
    public class DetailsModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public bool IsOwner { get; set; }

        public bool IsSubscribedUser { get; set; }


        public DetailsModel(ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        public Subscription Subscription { get; set; }
        public CoreService CoreService { get; }

        public VetMember[] SubscribedMembers { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subscription = await DbContext.Subscriptions
                .Include(c => c.VetMember)
                .Include(c => c.SubscriptionMembers)
                .AsQueryable()
                .FirstOrDefaultAsync(m => m.Id == id);

            var userContext = CoreService.GetUserContext();
            IsOwner = Subscription.VetMemberId == userContext.CurrentUser.Id;


            SubscribedMembers = await DbContext.SubscriptionMembers
                .AsQueryable()
                .Where(c => c.SubscriptionId == id)
                .Select(c => c.VetMember)
                .ToArrayAsync();


            IsSubscribedUser = DbContext.SubscriptionMembers.Any(c => c.SubscriptionId == id && c.VetMemberId == userContext.CurrentUser.Id);

            if (Subscription == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, string mode)
        {
            switch (mode)
            {
                case "subscribe":
                    return await Subscribe(id);
                case "unsubscribe":
                    return await Unsubscribe(id);

                default:
                    break;
            }

            return await OnGetAsync(id);
        }

        public async Task<IActionResult> Subscribe(int id)
        {
            var userContext = CoreService.GetUserContext();
            var subscription = DbContext.Subscriptions
                .Include(c => c.VetMember)
                .FirstOrDefault(c => c.Id == id);

            DbContext.SubscriptionMembers.Add(new SubscriptionMember
            {
                SubscriptionId = id,
                VetMemberId = userContext.CurrentUser.Id
            });
            await DbContext.SaveChangesAsync();

            await CoreService.SendDirectMessage(new[] { subscription.VetMember }, $@"{userContext.CurrentUser.Name}さんが{Subscription.Title}に登録されました");

            return await OnGetAsync(id);
        }

        public async Task<IActionResult> Unsubscribe(int id)
        {
            var userContext = CoreService.GetUserContext();
            var ss = DbContext.Subscriptions
                .Include(c => c.VetMember)
                .FirstOrDefault(c => c.Id == id);


            var sm = DbContext.SubscriptionMembers.First(c => c.SubscriptionId == id && c.VetMemberId == userContext.CurrentUser.Id);
            DbContext.SubscriptionMembers.Remove(sm);
            await DbContext.SaveChangesAsync();


            await CoreService.SendDirectMessage(new[] { ss.VetMember }, $@"{userContext.CurrentUser.Name}さんが{Subscription.Title}の登録を解除しました");

            return await OnGetAsync(id);
        }

    }
}
