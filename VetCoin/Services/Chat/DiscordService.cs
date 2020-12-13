using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
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
            WebRequestError,
            CrowdFundingNotification,
        }

        string GetChannelUrl(Channel chanel)
        {
            switch (chanel)
            {
                case Channel.TradeEntryNotification:
                    return Configuration.GetValue<string>("DiscordWebhookTradeEntryNotification");

                case Channel.ScheduleError:
                    return Configuration.GetValue<string>("DiscordWebhookScheduleError");
                case Channel.WebRequestError:
                    return Configuration.GetValue<string>("DiscordWebhookWebRequestError");
                case Channel.CrowdFundingNotification:
                    return Configuration.GetValue<string>("DiscordWebhookCrowdFundingNotification");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task SendMessage(Channel channel, string msg, DiscordEmbed embed = null)
        {
            try
            {
                var hc = HttpClientFactory.CreateClient();

                string json;

                if(msg.Length >= 1900)
                {
                    msg = msg.Substring(0, 1900);
                }

                
                if(embed == null)
                {
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        content = msg
                    });
                }
                else
                {

                    json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        content = msg,
                        embeds = new[]
                        {
                        embed,
                    }
                    });
                }

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

        public async Task SendError(string msg)
        {
            var hc = HttpClientFactory.CreateClient();
            string json;

            if (msg.Length >= 1900)
            {
                msg = msg.Substring(0, 1900);
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    content = msg
                });
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var url = GetChannelUrl(Channel.WebRequestError);
                if (string.IsNullOrEmpty(url))
                {
                    return;
                }
                var mpc = new MultipartFormDataContent();
                mpc.Add(content);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                ms.Write( System.Text.Encoding.UTF8.GetBytes(msg) );
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new StreamContent(ms);
                mpc.Add(sc, "file", "Error.txt");


                await hc.PostAsync(url, mpc);

            }
            else
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    content = msg
                });
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var url = GetChannelUrl(Channel.WebRequestError);

                if (string.IsNullOrEmpty(url))
                {
                    return;
                }
                await hc.PostAsync(url, content);
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
