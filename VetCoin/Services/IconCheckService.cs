using Discord;
using Discord.Rest;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VetCoin.Data;

namespace VetCoin.Services
{
    public class IconCheckService
    {
        public IconCheckService(ApplicationDbContext dbContext
            , IHttpClientFactory httpClientFactory
            , IConfiguration configuration)
        {
            DbContext = dbContext;
            HttpClientFactory = httpClientFactory;
            Configuration = configuration;
        }

        public ApplicationDbContext DbContext { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public IConfiguration Configuration { get; }

        public async Task IconCheck()
        {
            var members = DbContext.VetMembers
                .AsQueryable()
                .Where(c=>c.MemberType == MemberType.User)
                .ToArray();

            var hc = HttpClientFactory.CreateClient("IconCheckService");

            string token = Configuration.GetValue<string>("DiscordBotToken");
            var _rclient = new DiscordRestClient(new DiscordRestConfig { });
            await _rclient.LoginAsync(TokenType.Bot, token);

            foreach (var member in members)
            {
                var url = member.GetAvaterIconUrl();

                try
                {
                    var result = await hc.GetAsync(url);

                    if(result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        //Console.WriteLine();
                        var discordUser = await _rclient.GetUserAsync(member.DiscordId);
                        if (discordUser != null)
                        {
                            member.AvatarId = discordUser.AvatarId;
                        }
                        else
                        {
                            Console.WriteLine();
                        }

                    }
                    else
                    {
                        if(result.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine();
                        }
                    }

                }
                catch(HttpRequestException hre)
                {
                    Console.WriteLine(hre.ToString()); ;
                }
            }
            await DbContext.SaveChangesAsync();
        }
    }
}
