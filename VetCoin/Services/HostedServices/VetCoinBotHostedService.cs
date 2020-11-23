using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using ImageMagick;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VetCoin.Codes;
using VetCoin.Data;

namespace VetCoin.Services.HostedServices
{
    public class VetCoinBotHostedService : BackgroundService
    {
        private DiscordSocketClient _client;
        private DiscordRestClient _rclient;
        public VetCoinBotHostedService(IServiceProvider services, IConfiguration configuration, ILogger<VetCoinBotHostedService> logger, Codes.SiteContext siteContext)
        {
            Services = services;
            Configuration = configuration;
            Logger = logger;
            SiteContext = siteContext;
        }

        public IServiceProvider Services { get; }
        public IConfiguration Configuration { get; }

        public ILogger<VetCoinBotHostedService> Logger { get; }
        public SiteContext SiteContext { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });
            _client.MessageReceived += CommandRecieved;
            _client.ReactionAdded += _client_ReactionAdded;
            _client.UserVoiceStateUpdated += _client_UserVoiceStateUpdated;

            _rclient = new DiscordRestClient(new DiscordRestConfig { });

            //次の行に書かれているstring token = "hoge"に先程取得したDiscordTokenを指定する。
            string token = Configuration.GetValue<string>("DiscordBotToken");

            //トークンが取得出来ない場合、BOTは活動しません。
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await _rclient.LoginAsync(TokenType.Bot, token);

        }


        private async Task _client_UserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            await Task.Yield();
            Console.WriteLine();
        }

        private async Task _client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var amount = 0;
            //if (arg3.Emote.Name == "10")
            //{
            //    amount = 10;
            //}

            //if (arg3.Emote.Name == "50")
            //{
            //    amount = 50;
            //}

            //if (arg3.Emote.Name == "100vec")
            //{
            //    amount = 100;
            //}

            //if (arg3.Emote.Name == "500vec")
            //{
            //    amount = 500;
            //}

            var reactionMap = SiteContext.ReactionMaps.FirstOrDefault(c => c.Name == arg3.Emote.Name);
            if(reactionMap == null)
            {
                return;
            }

            amount = reactionMap.Amount;

            if (amount == 0)
            {
                return;
            }

            var message = await arg2.GetMessageAsync(arg3.MessageId);

            var toId = message.Author.Id;
            var fromId = arg3.UserId;

            //if(toId == fromId)
            //{
            //    return;
            //}

            IDMChannel fromDmChannel = null;
            if (arg3.User.IsSpecified)
            {
                fromDmChannel = await arg3.User.Value.GetOrCreateDMChannelAsync();
            }
            else
            {
                var user = await _rclient.GetUserAsync(fromId);
                fromDmChannel = await user.GetOrCreateDMChannelAsync();
            }
            var toDmChannel = await message.Author.GetOrCreateDMChannelAsync();



            using (var scope = Services.CreateScope())
            {
                var junmUrl = message.GetJumpUrl();
                var service = ActivatorUtilities.CreateInstance<ReactionSendService>(scope.ServiceProvider);
                await service.SendCoin(fromId, toId, amount, fromDmChannel, toDmChannel, junmUrl);
            }


            //arg2.GetMessageAsync()
            await Task.Yield();
            Console.WriteLine();
        }

        /// <summary>
        /// 何かしらのメッセージの受信
        /// </summary>
        /// <param name="msgParam"></param>
        /// <returns></returns>
        private async Task CommandRecieved(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            //デバッグ用メッセージを出力
            //Console.WriteLine("{0} {1}:{2}", message.Channel.Name, message.Author.Username, message);
            //メッセージがnullの場合
            if (message == null)
                return;

            //発言者がBotの場合無視する
            if (message.Author.IsBot)
                return;

            //ここから記述--------------------------------------------------------------------------
            var CommandContext = message.Content;

            // コマンド("おはよう")かどうか判定
            if (CommandContext.StartsWith("!superchat"))
            {
                //  await message.Channel.SendMessageAsync($"SuperChate @zio To @VetCoin 1000Ven");
                //  await message.Channel.SendMessageAsync($"スーパーチャットテストメッセージ");

                RegexOptions options = RegexOptions.Singleline;
                Regex regex = new Regex(@"!superchat[\s　]+<@!?(\d+)>[\s　]+(\d+)[\s　]?(.*)?", options);

                var fromDmChannel = await message.Author.GetOrCreateDMChannelAsync();
                var m = regex.Match(CommandContext);
                if (m.Success)
                {


                    var amount = int.Parse(m.Groups[2].Value);
                    var messageText = m.Groups[3].Value;
                    var toId = message.MentionedUsers.First().Id;

                    var targetUser = message.MentionedUsers.First();

                    IDMChannel toDmChannel = null;
                    try
                    {

                        toDmChannel = await targetUser.GetOrCreateDMChannelAsync();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine();
                    }


                    using (var scope = Services.CreateScope())
                    {
                        var service = ActivatorUtilities.CreateInstance<SuperChatService>(scope.ServiceProvider);
                        await service.PostSuperChat(message.Channel, message.Author.Id, toId, amount, messageText, fromDmChannel, toDmChannel);
                    }
                }
                else
                {
                    await fromDmChannel.SendMessageAsync(@"入力がおかしいです。下記形式で入力してください。
!superchat @user 100 Message

単一の対象に対して送信できます。
100～50000までの数字を指定してください。
");

                }

                Logger.LogInformation(CommandContext);

            }
            //  await message.Channel.SendFileAsync(@"C:\Users\info\source\repos\ConsoleApp14\ConsoleApp14\TEST.png", "AIUEOKAKIKUKEOK");
        }

    }

    public class SuperChatService
    {
        public SuperChatService(ApplicationDbContext dbContext, CoreService coreService, SiteContext siteContext)
        {
            DbContext = dbContext;
            CoreService = coreService;
            SiteContext = siteContext;
        }

        public ApplicationDbContext DbContext { get; }
        public CoreService CoreService { get; }
        public SiteContext SiteContext { get; }

        public async Task PostSuperChat(ISocketMessageChannel channel, ulong fromId, ulong toId, int amount, string message, IDMChannel fromDmChannel, IDMChannel toDmChannel)
        {
            var fromMember = DbContext.VetMembers.FirstOrDefault(c => c.DiscordId == fromId);
            if (fromMember == null)
            {
                await fromDmChannel.SendMessageAsync("VetCoin 登録者以外はSuperChatを送信できません");
                return;
            }

            var fromAmount = CoreService.CalcAmount(fromMember);
            if (fromAmount < amount)
            {
                await fromDmChannel.SendMessageAsync($"{SiteContext.CurrenryUnit}残高が不足しています。({fromAmount}{SiteContext.CurrenryUnit})");
                return;
            }

            if (amount < SiteContext.SuperChatLowLimit)
            {
                await fromDmChannel.SendMessageAsync($"送金下限は{SiteContext.SuperChatLowLimit}{SiteContext.CurrenryUnit}です。それ未満は送れません");
                return;
            }


            if (amount > SiteContext.SuperChatHeightLimit)
            {
                await fromDmChannel.SendMessageAsync($"送金上限は{SiteContext.SuperChatHeightLimit}{SiteContext.CurrenryUnit}です。それ以上は送れません");
                return;
            }

            var toMember = DbContext.VetMembers.FirstOrDefault(c => c.DiscordId == toId);
            if (toMember == null)
            {
                await fromDmChannel.SendMessageAsync("VetCoin 登録者以外へはSuperChatを送信できません");
                return;
                //toMember = DbContext.VetMembers.FirstOrDefault(c => c.DiscordId == 287434171570192384);
            }

            var toAmount = CoreService.CalcAmount(toMember);
            if (toMember.Id == fromMember.Id)
            {   //念のため同一人物の対応
                toAmount -= amount;
                fromAmount += amount;
            }

            try
            {
                if (fromDmChannel != null)
                {
                    await fromDmChannel.SendMessageAsync($@"SuperChat:{toMember.Name}へ{amount}{SiteContext.CurrenryUnit} 送金しました [{fromAmount - amount}{SiteContext.CurrenryUnit}]");
                }

                if(toDmChannel != null)
                {
                    await toDmChannel.SendMessageAsync($@"SuperChat:{fromMember.Name}から{amount}{SiteContext.CurrenryUnit} をもらいました [{toAmount + amount}{SiteContext.CurrenryUnit}]");
                }                

                DbContext.CoinTransactions.Add(new CoinTransaction
                {
                    SendeVetMemberId = fromMember.Id,
                    Amount = amount,
                    RecivedVetMemberId = toMember.Id,
                    Text = message,
                    TransactionType = CoinTransactionType.SuperChat,
                });
                await DbContext.SaveChangesAsync();
            }
            catch
            {
                await fromDmChannel.SendMessageAsync("システムトラブルの可能性があります。開発者に問い合わせをお願いします。");
                return;
            }

            try
            {
                var imageMs = await CreateImage(fromMember, toMember, amount);
                await channel.SendFileAsync(imageMs, $"Send{amount}.png", message);
            }
            catch
            {
                await fromDmChannel.SendMessageAsync("システムトラブルの可能性があります。開発者に問い合わせをお願いします。(送金は成功しています)");
            }
        }

        public async Task<string> GetImagePath(VetMember member)
        {
            HttpClient hc = new HttpClient();
            var tmpFilePath = Path.GetTempPath();
            var imageFilePath = Path.Combine(tmpFilePath, $"{member.DiscordId}_{member.AvatarId}.png");

            if (!File.Exists(imageFilePath))
            {
                var url = $"https://cdn.discordapp.com/avatars/{member.DiscordId}/{member.AvatarId}.png?size=128";
                var bytes = await hc.GetByteArrayAsync(url);

                await File.WriteAllBytesAsync(imageFilePath, bytes);
            }
            return imageFilePath;
        }

        public async Task<MemoryStream> CreateImage(VetMember from, VetMember to, int amount)
        {
            var colorCode = AmountToColor(amount);

            //HttpClientBuilderExtensions
            HttpClient hc = new HttpClient();


            //キャンバス画像を作成 100*50pixel
            using (var myMagick = new ImageMagick.MagickImage(new ImageMagick.MagickColor(colorCode), 640, 128))
            {
                //var  hc.GetByteArrayAsync($"https://cdn.discordapp.com/avatars/{from.DiscordId}/{from.AvatarId}.png?size=128"));
                var tmpFilePath = Path.GetTempPath();
                var fromImageFilePath = await GetImagePath(from);
                var toImageFilePath = await GetImagePath(to);

                using (var icon = new ImageMagick.MagickImage(fromImageFilePath))
                {
                    myMagick.Composite(icon);
                }

                using (var icon = new ImageMagick.MagickImage(toImageFilePath))
                {
                    myMagick.Composite(icon, new PointD(640 - 128, 0));
                }

                new Drawables()
              // Draw text on the image
              .FontPointSize(60)
              .Font("Comic Sans")
              .StrokeColor(MagickColors.White)
              .FillColor(MagickColors.White)
              //.TextAlignment(TextAlignment.Center)
              .Text(158, 64, $"{amount:#,0}{SiteContext.CurrenryUnit}")
              // Add an ellipse
              .StrokeColor(new MagickColor(0, Quantum.Max, 0))
              //.FillColor(MagickColors.SaddleBrown)
              //.Ellipse(256, 96, 192, 8, 0, 360)
              .Draw(myMagick);

                MemoryStream ms = new MemoryStream();
                myMagick.Write(ms, MagickFormat.Png);

                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }


        private string AmountToColor(int amount)
        {
            if (amount < 200)
            {
                return "#1565C0";
            }
            else if (amount < 500)
            {
                return "#80FFFF";
            }
            else if (amount < 1000)
            {
                return "#00BFA5";

            }
            else if (amount < 2000)
            {

                return "#FFB300";
            }
            else if (amount < 5000)
            {

                return "#E65100";
            }
            else if (amount < 10000)
            {

                return "#C2185B";
            }
            else
            {
                return "#D00000";
            }
        }
    }

    public class ReactionSendService
    {
        public ReactionSendService(ApplicationDbContext dbContext, CoreService coreService, SiteContext siteContext)
        {
            DbContext = dbContext;
            CoreService = coreService;
            SiteContext = siteContext;
        }

        public ApplicationDbContext DbContext { get; }
        public CoreService CoreService { get; }
        public SiteContext SiteContext { get; }

        public async Task SendCoin(ulong fromId, ulong toId, int amount, IDMChannel fromDmChannel, IDMChannel toDmChannel, string jumpUrl)
        {
            var fromMember = DbContext.VetMembers.FirstOrDefault(c => c.DiscordId == fromId);
            if (fromMember == null)
            {
                await fromDmChannel.SendMessageAsync($"{SiteContext.SiteTitle} 登録者以外は送信できません");
                return;
            }

            var fromAmount = CoreService.CalcAmount(fromMember);
            if (fromAmount < amount)
            {
                await fromDmChannel.SendMessageAsync($"{SiteContext.CurrenryUnit}残高が不足しています。({fromAmount}{SiteContext.CurrenryUnit})");
                return;
            }

            var toMember = DbContext.VetMembers.FirstOrDefault(c => c.DiscordId == toId);
            if (toMember == null)
            {
                await fromDmChannel.SendMessageAsync($"{SiteContext.SiteTitle} 登録者以外へは送信できません");
                return;
                //toMember = DbContext.VetMembers.FirstOrDefault(c => c.DiscordId == 287434171570192384);
            }

            var toAmount = CoreService.CalcAmount(toMember);

            if (toMember.Id == fromMember.Id)
            {   //念のため同一人物の対応
                toAmount -= amount;
                fromAmount += amount;
            }

            try
            {
                DbContext.CoinTransactions.Add(new CoinTransaction
                {
                    SendeVetMemberId = fromMember.Id,
                    Amount = amount,
                    RecivedVetMemberId = toMember.Id,
                    Text = "リアクション送金",
                    TransactionType = CoinTransactionType.ReactionSend,
                });
                await DbContext.SaveChangesAsync();
                if (fromDmChannel != null)
                {
                    await fromDmChannel.SendMessageAsync($@"Reaction:{toMember.Name} へ {amount} {SiteContext.CurrenryUnit} を送金しました[{fromAmount - amount}vec]");
                }
                await toDmChannel.SendMessageAsync($@"Reaction: {fromMember.Name} から {amount} {SiteContext.CurrenryUnit} をもらいました[{toAmount + amount}vec]
{jumpUrl}");
            }
            catch
            {
                await fromDmChannel.SendMessageAsync("システムトラブルの可能性があります。開発者に問い合わせをお願いします。");
                return;
            }
        }

    }


}
