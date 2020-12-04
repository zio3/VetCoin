using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VetCoinWasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            if (builder.HostEnvironment.BaseAddress == "https://localhost:44313/")
            {
                builder.RootComponents.Add<App>("#app");
            }

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<Api.ITradeLikeVotesClient>(sp => new Api.TradeLikeVotesClient(builder.HostEnvironment.BaseAddress, new HttpClient()));
            builder.Services.AddScoped<Api.IDonateLikeVotesClient>(sp => new Api.DonateLikeVotesClient(builder.HostEnvironment.BaseAddress, new HttpClient()));

            await builder.Build().RunAsync();
        }
    }
}
