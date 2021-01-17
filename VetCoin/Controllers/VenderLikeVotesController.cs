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
using VetCoin.Data.VenerEntityes;
using VetCoin.Services;

namespace VetCoin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenderLikeVotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VenderLikeVotesController(ApplicationDbContext context, CoreService coreService)
        {
            _context = context;
            CoreService = coreService;
        }

        public CoreService CoreService { get; }


        // POST: api/VenderLikeVotes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<VoteResult>> PostVenderLikeVote(int venderId)
        {
            //_context.VenderLikeVotes.Add(venderLikeVote);
            //CoreService.GetCu
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
            
            var vetMemberId = uc.CurrentUser.Id;

            var isVoted = false;
            var existItem = _context.VenderLikeVotes.FirstOrDefault(c => c.VenderId == venderId && c.VetMemberId == vetMemberId);
            if (existItem == null)
            {
                _context.VenderLikeVotes.Add(new VenderLikeVote
                {
                    VenderId = venderId,
                    VetMemberId = vetMemberId
                });
                isVoted = true;
            }
            else
            {
                _context.VenderLikeVotes.Remove(existItem);
                isVoted = false;
            }

            await _context.SaveChangesAsync();

            var count = await _context.VenderLikeVotes
                .AsQueryable()
                .CountAsync(c => c.VenderId == venderId);

            return new VoteResult
            {
                Count = count,
                IsVoted = isVoted
            };
        }


        private bool VenderLikeVoteExists(int id)
        {
            return _context.VenderLikeVotes.Any(e => e.Id == id);
        }
    }

    //public class VoteResult
    //{
    //    public int Count { get; set; }
    //    public bool IsVoted{ get; set; }
    //}
}
