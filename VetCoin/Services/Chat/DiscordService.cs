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

        public async Task SendMessage(Channel channel, string msg, DiscordEmbed embed = null)
        {
            try
            {
                var hc = HttpClientFactory.CreateClient();

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    content = msg,
                    embeds = new[]
                    {
                        embed,
                    }
                });

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var url = GetChannelUrl(channel);

                if (string.IsNullOrEmpty(url))
                {
                    return;
                }
                await hc.PostAsync(url, content);
            }
            catch
            {

            }
        }



        public class DiscordEmbed
        {
            public string color { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public Author author { get; set; }
            public string description { get; set; }
            public Thumbnail thumbnail { get; set; }
            public Field[] fields { get; set; }
            public Image image { get; set; }
            public object timestamp { get; set; }
            public Footer footer { get; set; }

            public class Author
            {
                public string name { get; set; }
                public string icon_url { get; set; }
                public string url { get; set; }
            }

            public class Thumbnail
            {
                public string url { get; set; }
            }

            public class Image
            {
                public string url { get; set; }
            }

            public class Footer
            {
                public string text { get; set; }
                public string icon_url { get; set; }
            }

            public class Field
            {
                public string name { get; set; }
                public string value { get; set; }
                public bool inline { get; set; }
            }

        }




    }




}
