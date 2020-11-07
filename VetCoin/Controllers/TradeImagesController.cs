using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VetCoin.Data;

namespace VetCoin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeImagesController : ControllerBase
    {
        public TradeImagesController(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await DbContext.TradeImages.FindAsync(id);
            if (entity == null)
            {
                return Redirect("/noimage.gif");
            }

            var ms = new MemoryStream(entity.ImageContent);

            ms.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(ms, "application/octet-stream");

        }
    }
}
