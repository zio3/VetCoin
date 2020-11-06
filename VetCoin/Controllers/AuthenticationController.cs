using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Discord.Rest;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public ApplicationDbContext DbContext { get; }
        public CoreService CoreService { get; }

        public AuthenticationController(ApplicationDbContext dbContext, CoreService coreService)
        {
            DbContext = dbContext;
            CoreService = coreService;
        }

        //public SignInManager SignInManager { get; }

        //public AuthenticationController(SignInManager signInManager)
        //{
        //    SignInManager = signInManager;
        //}

        [HttpGet()]
        public async Task<IActionResult> SignInxAsync(string code = null)
        {
            var result = await CoreService.Authentication(code);

            if(!result.IsAuthenticated)
            {
                return RedirectToPage("/index");
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.User.Name),
                new Claim("AvatarId",result.User.AvatarId),
                new Claim("DiscordId",result.User.DiscordId.ToString()),
            };

            //必要に応じてUserを作る

            var claimsIdentity = new ClaimsIdentity(
                     claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(10)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/index");
        }

        public class TokenInfo
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            public string token_type { get; set; }
        }

    }
}
