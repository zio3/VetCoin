using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeLikeVotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TradeLikeVotesController(ApplicationDbContext context, CoreService coreService)
        {
            _context = context;
            CoreService = coreService;
        }

        public CoreService CoreService { get; }


        // POST: api/TradeLikeVotes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<VoteResult>> PostTradeLikeVote(int tradeId)
        {
            //_context.TradeLikeVotes.Add(tradeLikeVote);
            //CoreService.GetCu
            Stopwatch sw = new Stopwatch();
            sw.Start();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{sw.Elapsed}");
            var uc = CoreService.GetUserContext();
#if DEBUG
            if (uc == null)
            {
                uc = new UserContext
                {
                    CurrentUser = new VetMember { Id = 3 }
                };
            }
#endif
            sb.AppendLine($"{sw.Elapsed}");

            var vetMemberId = uc.CurrentUser.Id;

            var isVoted = false;
            sb.AppendLine($"{sw.Elapsed}");
            var existItem = _context.TradeLikeVotes.FirstOrDefault(c => c.TradeId == tradeId && c.VetMemberId == vetMemberId);
            sb.AppendLine($"{sw.Elapsed}");
            if (existItem == null)
            {
                sb.AppendLine($"{sw.Elapsed}");
                _context.TradeLikeVotes.Add(new TradeLikeVote
                {
                    TradeId = tradeId,
                    VetMemberId = vetMemberId
                });
                isVoted = true;
            }
            else
            {
                sb.AppendLine($"{sw.Elapsed}");
                _context.TradeLikeVotes.Remove(existItem);
                isVoted = false;
            }
            sb.AppendLine($"{sw.Elapsed}");

            await _context.SaveChangesAsync();
            sb.AppendLine($"{sw.Elapsed}");

            var count = await _context.TradeLikeVotes
                .AsQueryable()
                .CountAsync(c => c.TradeId == tradeId);

            sb.AppendLine($"{sw.Elapsed}");
            //return CreatedAtAction("GetTradeLikeVote", new { id = tradeLikeVote.Id }, tradeLikeVote);
            var x = sb.ToString();
            return new VoteResult
            {
                Count = count,
                IsVoted = isVoted
            };
        }


        private bool TradeLikeVoteExists(int id)
        {
            return _context.TradeLikeVotes.Any(e => e.Id == id);
        }
    }

    public class VoteResult
    {
        public int Count { get; set; }
        public bool IsVoted{ get; set; }
    }
}
