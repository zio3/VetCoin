using CsvHelper;
using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VetCoin.Codes;
using VetCoin.Data;

namespace VetCoin.Services
{
    public class CoreService
    {
        const ulong VET_GUILD_ID = 627003812892114985;
        const ulong STUB_USER_ID = 999999999;

        public CoreService(IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext dbContext, 
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory)
        {
            HttpContextAccessor = httpContextAccessor;
            DbContext = dbContext;
            Configuration = configuration;
            HttpClientFactory = httpClientFactory;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }
        public ApplicationDbContext DbContext { get; }
        public IConfiguration Configuration { get; }
        public IHttpClientFactory HttpClientFactory { get; }

        public IQueryable<CoinTransaction> GetCoinTransactionQuery(VetMember vetMember)
        {
            return DbContext
                .CoinTransactions
                .AsQueryable()
                .Where(c => c.SendeVetMemberId == vetMember.Id || c.RecivedVetMemberId == vetMember.Id);
        }

        public long CalcAmount(VetMember member)
        {
            var sendAmount = DbContext.CoinTransactions
                    .AsQueryable()
                    .Where(c => c.SendeVetMemberId == member.Id)
                    .Sum(c => c.Amount);

            var reciveAmount = DbContext.CoinTransactions
                .AsQueryable()
                .Where(c => c.RecivedVetMemberId == member.Id)
                .Sum(c => c.Amount);

            return reciveAmount - sendAmount;
        }

        public async Task<(bool isVetMember, RestSelfUser user)> GetVetMember(string accessToken)
        {
            var dc = new DiscordRestClient();
            await dc.LoginAsync(Discord.TokenType.Bearer, accessToken);
            var gs = dc.GetGuildSummariesAsync();
            var isVetMember = (await gs.ToArrayAsync()).SelectMany(c => c).Any(c => c.Id == VET_GUILD_ID);

            return (isVetMember, dc.CurrentUser);
        }

        public async Task<VetMember> JoinUser(RestSelfUser user)
        {
            var addUser = new VetMember
            {
                DiscordId = user.Id,
                AvatarId = user.AvatarId,
                Name = user.Username
            };
            var issuer = await DbContext.VetMembers
                .AsQueryable()
                .FirstOrDefaultAsync(c => c.MemberType == MemberType.Issuer);
            var vault = await DbContext                
                .VetMembers
                .AsQueryable().FirstOrDefaultAsync(c => c.MemberType == MemberType.Vault);

 

            

            //var initTran = new CoinTransaction
            //{
            //    Amount = 4000,
            //    SendVetMember = vault,
            //    RecivedVetMember = addUser,
            //    Text = "[初期配布コイン]",
            //    TransactionType = CoinTransactionType.InitialDistribution,
            //};

            DbContext.VetMembers.Add(addUser);
            if (DbContext.CoinTransactions.Count() != 0)
            {
                var issueTran = new CoinTransaction
                {
                    Amount = 100000,
                    SendVetMember = issuer,
                    RecivedVetMember = vault,
                    TransactionType = CoinTransactionType.Issue,
                    Text = $"[登録時発行:{user.Username}]"
                };
                DbContext.CoinTransactions.Add(issueTran);
            }
            //DbContext.CoinTransactions.Add(initTran);
            return addUser;
        }


        public UserContext GetUserContext()
        {
            var httpContext = HttpContextAccessor.HttpContext;

            var discordIdStr = httpContext.User.Claims.First(c => c.Type == "DiscordId").Value;
            if (!ulong.TryParse(discordIdStr, out ulong discordId))
            {
                return null;
            }
            var sender = DbContext.VetMembers.FirstOrDefault(c => c.DiscordId == discordId);
            if (sender == null)
            {
                return null;
            }

            var amount = CalcAmount(sender);

            return new UserContext {
                CurrentUser = sender,
                Amount = amount
            };
        }

        internal void AddTransaction(UserContext userContext, long sendAmount, string message, int reciveVetMemberId,CoinTransactionType type)
        {
            DbContext.CoinTransactions.Add(new CoinTransaction
            {
                SendeVetMemberId = userContext.CurrentUser.Id,
                RecivedVetMemberId = reciveVetMemberId,
                Amount = sendAmount,
                Text = message,
                TransactionType = type
            });
        }

        public async Task SavechanesAsnc()
        {
            await DbContext.SaveChangesAsync();
        }

        public IEnumerable<SelectListItem> GetOtherUserDdl(VetMember currentUser )
        {
            return DbContext.VetMembers
             .AsQueryable()
             //.Where(c => c.Id != 1)
             .Where(c => c.Id != currentUser.Id)
             .Where(c => c.MemberType == MemberType.User)
             .OrderBy(c=>c.Name)
             .Select(c => new SelectListItem { Text = $"{c.Name}", Value = c.Id.ToString() });
        }

        public string GetLoginPageUrl()
        {
            var httpContext = HttpContextAccessor.HttpContext;
            var encoeeHostName = HttpUtility.UrlEncode(httpContext.Request.Host.ToString());
            return $"https://discord.com/api/oauth2/authorize?client_id=756087315440599050&redirect_uri=https%3A%2F%2F{encoeeHostName}%2Fapi%2FAuthentication&response_type=code&scope=identify%20guilds";
        }

        public string GetRedirectUrl()
        {
            var httpContext = HttpContextAccessor.HttpContext;

            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/Authentication";

        }

        public string CsvExportAllTransactions()
        {
            var recodes = DbContext.CoinTransactions.AsQueryable().Select(c => new CoinTransactionsCsvrow
            {
                Id = c.Id,
                DateTime = c.UpdateDate.ToOffset(Consts.JstOffset).DateTime,
                Type = c.TransactionType.ToString(),
                SnendMemberId = c.SendVetMember.DiscordId,
                SnendMemberName = c.SendVetMember.Name,
                ReciveMemberId = c.RecivedVetMember.DiscordId,
                ReciveMemberName = c.RecivedVetMember.Name,
                Amount = c.Amount,
                Text = c.Text
            });

            return ExportCsv(recodes);
        }

        public async Task SendDirectMessage(VetMember[] members,string message)
        {
            var rclient = new DiscordRestClient(new DiscordRestConfig { });
            string token = Configuration.GetValue<string>("DiscordBotToken");
            //トークンが未設定の場合、何も起こらなくする
            if(string.IsNullOrEmpty(token))
            {
                return;
            }


            await rclient.LoginAsync(TokenType.Bot, token);

            foreach (var member in members)
            {
                var user = await rclient.GetUserAsync(member.DiscordId);
                var dmc = await user.GetOrCreateDMChannelAsync();
                await dmc.SendMessageAsync(message);
            }
        }

        private string ExportCsv<T>(IEnumerable<T> src)
        {
            var sb = new StringBuilder();
            using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
            {
                var config = csv.Configuration;
                config.HasHeaderRecord = true; // ヘッダーが存在する場合 true

                // 区切り文字をタブとかに変えることも可能
                //config.Delimiter = "\t";

                csv.WriteRecords(src);
            }
            return sb.ToString();
        }
        public int TotalDistibutionAmount()
        {
            var userCount = DbContext.VetMembers.Count(c => c.MemberType == MemberType.User);
            //現在は仮で (人数 * 50000 /12)

            //12ヶ月経過後毎の半減をあとで組み入れる

            return userCount * 50000 / 12;
        }

        public async Task<MemberDistributeAmount[]> CalcMemberDistributeAmount(int totalAmount)
        {
            return await DbContext
                .VetMembers
                .AsQueryable()
                .Where(c => c.MemberType == MemberType.User)
                .Select(c =>new MemberDistributeAmount
                {
                    VetMember = c,
                    Amount = 4000
                }).ToArrayAsync();
        }

        public async Task<AuthenticationResult> StubAuthentication()
        {
            var entity = DbContext.VetMembers.FirstOrDefault(c => c.DiscordId == STUB_USER_ID);
            if (entity == null)
            {
                entity = new VetMember
                {
                    DiscordId = STUB_USER_ID,
                    AvatarId = string.Empty,
                    Name = "[STUB_USER]"
                };
                DbContext.VetMembers.Add(entity);
            }
            await DbContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                IsAuthenticated = true,
                User = entity
            };
        }


        public async Task<AuthenticationResult> Authentication(string code)
        {
            var hc = HttpClientFactory.CreateClient("DiscordApp");

            var clientSecret = Configuration.GetValue<string>("DiscordClientSeclet");

            if(string.IsNullOrEmpty(clientSecret))
            {
                return await StubAuthentication();
            }

            var form = new Dictionary<string, string>();
            form.Add("client_id", "756087315440599050");
            form.Add("client_secret", clientSecret);
            form.Add("grant_type", "authorization_code");
            form.Add("redirect_uri", GetRedirectUrl());
            form.Add("code", code);
            form.Add("scope ", "identify guilds webhook.incoming");

            var formContent = new FormUrlEncodedContent(form);

            var c = await hc.PostAsync("https://discordapp.com/api/oauth2/token", formContent);

            var json = await c.Content.ReadAsStringAsync();

            var tokenInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscordTokenTokenInfo>(json);

            //該当ユーザーがVETメンバーなのかのチェック
            var memberInfo = await GetVetMember(tokenInfo.access_token);
            if (!memberInfo.isVetMember)
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false
                };
            }
            var user = memberInfo.user;

            var entity = DbContext.VetMembers.FirstOrDefault(c => c.DiscordId == user.Id);
            if (entity == null)
            {
                entity = await JoinUser(user);
            }
            else
            {
                entity.AvatarId = user.AvatarId;
                entity.DiscordId = user.Id;
                entity.Name = user.Username;
            }

            DbContext.SaveChanges();

            return new AuthenticationResult
            {
                IsAuthenticated = true,
                User = entity
            };

        }

        public async Task RegularDistribution()
        {
            //.出金の総額を算出
            //現在は仮で (人数 * 50000 /12)
            var totalAmount = TotalDistibutionAmount();

            //.数値算出のあるご作成
            var mdas = await CalcMemberDistributeAmount(totalAmount);

            //2.発行済保管庫から、取り出す
            var vault = await DbContext.VetMembers.AsQueryable().Where(c => c.MemberType == MemberType.Vault).FirstAsync();
            foreach (var mda in mdas)
            {
                DbContext.CoinTransactions.Add(new CoinTransaction
                {
                    Amount = mda.Amount,
                    SendeVetMemberId = vault.Id,
                    RecivedVetMemberId = mda.VetMember.Id,
                    TransactionType = CoinTransactionType.RegularDistribution,
                    Text = "[月次配布]",
                });
            }

            //3.あまりを銀行に入れる
            var totalDistributeAmount = mdas.Sum(c => c.Amount);
            var bank = await DbContext.VetMembers.AsQueryable().Where(c => c.MemberType == MemberType.Bank).FirstAsync();
            DbContext.CoinTransactions.Add(new CoinTransaction
            {
                Amount = totalAmount-totalDistributeAmount,
                SendeVetMemberId = vault.Id,
                RecivedVetMemberId = bank.Id,
                TransactionType = CoinTransactionType.RegularDistribution,
                Text = "[月次配布残額]",
            });

        }
    }

    public class MemberDistributeAmount
    {
        public VetMember VetMember { get; set; }
        public int Amount { get; set; }
    }


    public class UserContext
    {
        public VetMember CurrentUser { get; set; }
        public long Amount { get; set; }

    }

    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }

        public VetMember User{get;set;}
    }


    public class CoinTransactionsCsvrow
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Type { get; set; }

        

        public ulong SnendMemberId { get; set; }
        public string SnendMemberName { get; set; }


        public ulong ReciveMemberId { get; set; }
        public string ReciveMemberName { get; set; }

        public long Amount { get; set; }

        public string Text { get; set; }
    }

    public class DiscordTokenTokenInfo
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
    }



}
