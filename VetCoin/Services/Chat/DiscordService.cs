using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace VetCoin.Services.Chat
{
    public class DiscordService
    {
        public DiscordService(IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            HttpClientFactory = httpClientFactory;
            Configuration = configuration;
        }


        public IHttpClientFactory HttpClientFactory { get; }
        public IConfiguration Configuration { get; }

        public enum Channel
        {
            TradeEntryNotification,
            ScheduleError,
        }

        string GetChannelUrl(Channel chanel)
        {
            switch (chanel)
            {
                case Channel.TradeEntryNotification:
                    return Configuration.GetValue<string>("DiscordWebhookTradeEntryNotification");

                case Channel.ScheduleError:
                    return Configuration.GetValue<string>("DiscordWebhookScheduleError");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task SendMessage(Channel channel, string msg)
        {
            try
            {
                var hc = HttpClientFactory.CreateClient();

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    content = msg
                });

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var url = GetChannelUrl(channel);

                if(string.IsNullOrEmpty(url))
                {
                    return;
                }
                await hc.PostAsync(url, content);
            }
            catch
            {

            }
        }

    }
}
