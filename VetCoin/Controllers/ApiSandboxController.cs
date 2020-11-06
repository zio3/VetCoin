using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services.Chat;
using VetCoin.Services.HostedServices;

namespace VetCoin.Controllers
{
#if DEBUG
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiSandboxController : ControllerBase
    {
        public ApiSandboxController(SuperChatService superChatService, DiscordService discordService, Data.ApplicationDbContext applicationDbContext)
        {
            SuperChatService = superChatService;
            DiscordService = discordService;
            DbContext = applicationDbContext;
        }

        public SuperChatService SuperChatService { get; }
        public DiscordService DiscordService { get; }
        public Data.ApplicationDbContext DbContext { get; }

    }
#endif
}
