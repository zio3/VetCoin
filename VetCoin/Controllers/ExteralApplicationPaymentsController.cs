using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VetCoin.Codes;
using VetCoin.Data;
using VetCoin.Data.ExtApp;
using VetCoin.Services;
using VetCoin.Services.Chat;

namespace VetCoin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExteralApplicationPaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoreService CoreService { get; }
        public DiscordService DiscordService { get; }
        public SiteContext SiteContext { get; }

        public ExteralApplicationPaymentsController(ApplicationDbContext context, CoreService coreService,DiscordService discordService,SiteContext siteContext)
        {
            _context = context;
            CoreService = coreService;
            DiscordService = discordService;
            SiteContext = siteContext;
        }

        // GET: api/ExteralApplicationPayments
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ExteralApplicationPayment>>> GetExteralApplicationPayments()
        //{
        //    return await _context.ExteralApplicationPayments

        //        .ToListAsync();
        //}

        // GET: api/ExteralApplicationPayments/5
        [HttpGet]
        public async Task<ActionResult<ExteralApplicationPayment>> GetExteralApplicationPayment(Guid id, string discordId)
        {
            var discordIdUl = ulong.Parse(discordId);

            var exteralApplicationPayment = await _context.ExteralApplicationPayments
                .AsQueryable()
                .FirstOrDefaultAsync(c => c.Id == id && c.DiscordId == discordIdUl);

            if (exteralApplicationPayment == null)
            {
                return NotFound();
            }

            return exteralApplicationPayment;
        }

        // PUT: api/ExteralApplicationPayments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<PutResult>> PutExteralApplicationPayment(Guid id, string discordId)
        {
            var discordIdul = ulong.Parse(discordId);

            var exteralApplicationPayment = await _context.ExteralApplicationPayments
                    .Include(c=>c.ExteralApplication)
                    .AsQueryable()
                    .FirstOrDefaultAsync(c => c.Id == id && c.DiscordId == discordIdul);

            if (exteralApplicationPayment == null)
            {
                return NotFound();
            }

            if(exteralApplicationPayment.ExpirationDate >= DateTimeOffset.Now)
            {
                return new PutResult
                {
                    IsSucceed = false,
                    ErrorMessage = "有効期限切れ"
                };
            }

            //TODO:トランザクションを作る
            var venderId = exteralApplicationPayment.ExteralApplication.VetMemberId;
            var buyMember = await _context.VetMembers.AsQueryable().FirstOrDefaultAsync(c => c.DiscordId == discordIdul);
            var lestAmount = CoreService.CalcAmount(buyMember);

            if(lestAmount < exteralApplicationPayment.Amount)
            {
                return new PutResult
                {
                    IsSucceed = false,
                    ErrorMessage = "残高不足"
                };
            }

            var transaction = new CoinTransaction
            {
                SendeVetMemberId = buyMember.Id,
                RecivedVetMemberId = venderId,
                Amount = exteralApplicationPayment.Amount,
                Text = $"外部購入:{exteralApplicationPayment.ExteralApplication.Name}:{exteralApplicationPayment.Id}",
            };

            _context.CoinTransactions.Add(transaction);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExteralApplicationPaymentExists(id))
                {
                    return new PutResult
                    {
                        IsSucceed = false,
                        ErrorMessage = "DB書き込みに失敗しました。",
                    };
                }
                else
                {
                    throw;
                }
            }

            return new PutResult
            {
                IsSucceed = true,
            };
        }

        // POST: api/ExteralApplicationPayments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostResult>> PostExteralApplicationPayment(PostRequest postRequest)
        {
            var discordIdUl = ulong.Parse(postRequest.DiscordId);

            var buyMember = await _context.VetMembers.AsQueryable().FirstOrDefaultAsync(c => c.DiscordId == discordIdUl);
            if(buyMember == null)
            {
                return new PostResult
                {
                    IsSucceed = false,
                    ErrorMessage = "不正なユーザーのリクエストです"
                };
            }

            var lestAmount = CoreService.CalcAmount(buyMember);

            if (lestAmount < postRequest.Amount)
            {
                return new PostResult
                {
                    IsSucceed = false,
                    ErrorMessage = "残高不足です。"
                };
            }

            var app = await _context.ExteralApplications.AsQueryable()
                .Include(c=>c.VetMember)
                .FirstOrDefaultAsync(c => c.Id == postRequest.AppId);

  

            var entity = new ExteralApplicationPayment
            {
                ExteralApplicationId = postRequest.AppId,
                Amount = postRequest.Amount,
                Description = postRequest.Description,
                DiscordId = discordIdUl,
                RefJson = postRequest.RefJson,
                ExpirationDate = DateTimeOffset.Now.AddMinutes(5)
            };

            _context.ExteralApplicationPayments.Add(entity);
            await _context.SaveChangesAsync();

            //Todo:DMを送信する
            await SendConfirmNotificationAsync(app, buyMember, entity);

            return new PostResult
            {
                IsSucceed = true
            };
        }

        private async Task SendConfirmNotificationAsync(ExteralApplication app, VetMember user, ExteralApplicationPayment eap)
        {
            Discord.EmbedBuilder builder = new Discord.EmbedBuilder();

            builder.WithTitle($"{app.Name}:購入確認");
            builder.WithUrl(app.CallbackUrl + $"?id={eap.Id}");
            builder.WithAuthor(app.VetMember.Name, app.VetMember.GetAvaterIconUrl(), app.VetMember.GetMemberPageUrl(SiteContext.SiteBaseUrl));
            builder.AddField("金額", eap.Amount);
            builder.WithDescription(eap.Description);
            builder.WithFooter("よろしければ、タイトルをクリックして購入処理をつづけてください");

            await CoreService.SendDirectMessage(new[] { user }, string.Empty, builder.Build());
        }

        // DELETE: api/ExteralApplicationPayments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExteralApplicationPayment(Guid id)
        {
            var exteralApplicationPayment = await _context.ExteralApplicationPayments.FindAsync(id);
            if (exteralApplicationPayment == null)
            {
                return NotFound();
            }

            _context.ExteralApplicationPayments.Remove(exteralApplicationPayment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExteralApplicationPaymentExists(Guid id)
        {
            return _context.ExteralApplicationPayments.Any(e => e.Id == id);
        }

        public class PostRequest
        {
            public Guid AppId { get; set; }

            public int Amount { get; set; }

            public string Description { get; set; }

            public string DiscordId { get; set; }

            public string RefJson { get; set; }
        }
        public class PostResult
        {
            public bool IsSucceed { get; set; }

            public string ErrorMessage { get; set; }
        }

        public class PutResult
        {
            public bool IsSucceed { get; set; }

            public string ErrorMessage { get; set; }
        }
    }


}
