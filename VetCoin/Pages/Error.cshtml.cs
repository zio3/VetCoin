using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using VetCoin.Services.Chat;

namespace VetCoin.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public DiscordService DiscordService { get; }

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger, DiscordService discordService)
        {
            _logger = logger;
            DiscordService = discordService;
        }

        public async Task OnGetAsync()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            
            var error = HttpContext
                    .Features
                        .Get<IExceptionHandlerFeature>();


            //await DiscordService.SendMessage(DiscordService.Channel.WebRequestError, error.Error.ToString());
            await DiscordService.SendError(error.Error.ToString());
        }
    }
}
