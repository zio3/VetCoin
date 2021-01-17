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
        //const ulong VET_GUILD_ID = 627003812892114985;
        const ulong STUB_USER_ID = 999999999;

        public CoreService(IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext dbContext, 
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory,
            SiteContext siteContext
            , StaticSettings staticSettings)
        {
            HttpContextAccessor = httpContextAccessor;
            DbContext = dbContext;
            Configuration = configuration;
            HttpClientFactory = httpClientFactory;
            SiteContext = siteContext;
            StaticSettings = staticSettings;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }
        public ApplicationDbContext DbContext { get; }
        public IConfiguration Configuration { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public SiteContext SiteContext { get; }
        public StaticSettings StaticSettings { get; }

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
            var isVetMember = (await gs.ToArrayAsync()).SelectMany(c => c).Any(c => c.Id == StaticSettings.LoginCheckDiscordServerId);

            return (isVetMember, dc.CurrentUser);
        }

        public async Task<IEnumerable<Contract>> EnumWaitingContracts(VetMember vetMember )
        {
            if(vetMember == null)
            {
                return new Contract[0];
            }

            return await DbContext
                .Contracts
                .Include(c => c.VetMember)
                .Include(c => c.Trade.VetMember)
                .AsQueryable()
                .Where(c => c.ContractStatus == ContractStatus.Deliveryed)
                .Where(c =>
                    (c.Trade.Direction == Data.Direction.Buy && c.Trade.VetMemberId == vetMember.Id) ||
                    (c.Trade.Direction == Data.Direction.Sell && c.VetMemberId == vetMember.Id))
                .ToArrayAsync();
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
                //var issueTran = new CoinTransaction
                //{
                //    Amount = 100000,
                //    SendVetMember = issuer,
                //    RecivedVetMember = vault,
                //    TransactionType = CoinTransactionType.Issue,
                //    Text = $"[登録時発行:{user.Username}]"
                //};
                //DbContext.CoinTransactions.Add(issueTran);
            }
            //DbContext.CoinTransactions.Add(initTran);
            return addUser;
        }


        public UserContext GetUserContext()
        {
            var httpContext = HttpContextAccessor.HttpContext;

            var discordIdStr = httpContext.User.Claims.FirstOrDefault(c => c.Type == "DiscordId")?.Value??string.Empty;
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
            return $"https://discord.com/api/oauth2/authorize?client_id={StaticSettings.DiscordAppClientId}&redirect_uri=https%3A%2F%2F{encoeeHostName}%2Fapi%2FAuthentication&response_type=code&scope=identify%20guilds";
        }

        public string GetRedirectUrl()
        {
            var httpContext = HttpContextAccessor.HttpContext;

            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/Authentication";

        }

        public string CsvExportAllTransactions(bool exceptSamePersonTransaction)
        {
            var query = DbContext.CoinTransactions.AsQueryable();

            if(exceptSamePersonTransaction)
            {
                query = query.Where(c => c.SendeVetMemberId != c.RecivedVetMemberId);
            }

            var recodes = query.Select(c => new CoinTransactionsCsvrow
            {
                Id = c.Id,
                CreateDateTime = c.CreateDate.ToOffset(Consts.JstOffset).DateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                UpdateDateTime = c.UpdateDate.ToOffset(Consts.JstOffset).DateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                Type = c.TransactionType.ToString(),
                SendMemberId = c.SendVetMember.DiscordId,
                SendMemberName = c.SendVetMember.Name,
                ReciveMemberId = c.RecivedVetMember.DiscordId,
                ReciveMemberName = c.RecivedVetMember.Name,
                Amount = c.Amount,
                Text = c.Text
            });

            return ExportCsv(recodes);
        }

        public async Task SendDirectMessage(VetMember[] members,string message,Discord.Embed enbed = null)
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
                

                if (enbed == null)
                {
                    await dmc.SendMessageAsync(message);
                }
                else
                {
                    await dmc.SendMessageAsync(message,false,enbed);
                }

                
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
            //var userCount = DbContext.VetMembers.Count(c => c.MemberType == MemberType.User);
            ////現在は仮で (人数 * 50000 /12)

            ////12ヶ月経過後毎の半減をあとで組み入れる

            //return userCount * 50000 / 12;

            //月あたり100,000固定になりました。

            return 100000;
        }

        public async Task<MemberDistributeAmount[]> CalcMemberDistributeAmount(int totalAmount)
        {
            var members = await DbContext.VetMembers
                .AsQueryable()
                .Where(c => c.MemberType == MemberType.User)
                .ToArrayAsync();

            var trades = await DbContext.Trades
                .AsQueryable()
                .Include(c=>c.Contracts)
                .Where(c=>c.TradeStatus != TradeStatus.Cancel)
                .ToArrayAsync();

            //var targetMonth = DateTime.Today
            var limitDate = DateTime.Today;
            if(DateTime.Today.Day == 1)
            {
                //targetMonth = DateTime.Today.AddMonths(-2);
                limitDate = limitDate.AddDays(-1);
            }
            var limitStartDate = new DateTimeOffset(limitDate.Year, limitDate.Month, 1, 0, 0, 0, Consts.JstOffset);

            trades = trades.Where(c =>
            {
                if(c.IsContinued)
                {
                    return true;
                }

                //作業中があれば
                var wk = c.Contracts.Any(d => d.ContractStatus == ContractStatus.Working);
                if (wk == true)
                {
                    return true;
                }

                var newrComplite = c.Contracts.Any(d => d.ContractStatus == ContractStatus.Complete && d.UpdateDate >= limitStartDate);
                if (newrComplite == true)
                {
                    return true;
                }

                if(c.Contracts.Count() == 0)
                {
                    return true;
                }
                return false;
            }).ToArray();


            var baseDistribute = members.Count() * 500;
            var tradeDistribute = totalAmount - baseDistribute;

            var summary =  members.Select(c => new
            {
                Member = c,
                RawBuyTradeCount = trades.Count(d => d.VetMemberId == c.Id && d.Direction == Data.Direction.Buy),
                RawSellTradeCount = trades.Count(d => d.VetMemberId == c.Id && d.Direction == Data.Direction.Sell),
                BuyTradeCount = Math.Min(3, trades.Count(d => d.VetMemberId == c.Id && d.Direction == Data.Direction.Buy)),
                SellTradeCount = Math.Min(3, trades.Count(d => d.VetMemberId == c.Id && d.Direction == Data.Direction.Sell)),
            }).ToArray();

            var buyTradeTotal = summary.Sum(c => c.BuyTradeCount);
            var sellTradeTotal = summary.Sum(c => c.SellTradeCount);

            var tradeTotal = buyTradeTotal + sellTradeTotal;

            var results = summary.Select(c => new MemberDistributeAmount
            {
                VetMember = c.Member,
                RawBuyTradeCount = c.RawBuyTradeCount,
                RawSellTradeCount = c.RawSellTradeCount,
                BuyTradeCount = c.BuyTradeCount,
                SellTradeCount = c.SellTradeCount,
                Amount = (int)Math.Floor((double)tradeDistribute * (double)(c.BuyTradeCount + c.SellTradeCount) / (double)tradeTotal) + 500
            }).ToArray();

            return results;

            //return await DbContext
            //    .VetMembers
            //    .AsQueryable()
            //    .Where(c => c.MemberType == MemberType.User)
            //    .Select(c =>new MemberDistributeAmount
            //    {
            //        VetMember = c,
            //        Amount = 4000
            //    }).ToArrayAsync();
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
            form.Add("client_id", StaticSettings.DiscordAppClientId);
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

#if DEBUG
            entity = DbContext.VetMembers.Find(12);
#endif

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


        public int RawSellTradeCount { get; set; }
        public int RawBuyTradeCount { get; set; }

        public int SellTradeCount { get; set; }
        public int BuyTradeCount { get; set; }
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

        public string CreateDateTime { get; set; }

        public string UpdateDateTime { get; set; }

        public string Type { get; set; }

        

        public ulong SendMemberId { get; set; }
        public string SendMemberName { get; set; }


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
