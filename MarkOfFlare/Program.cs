using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MvvmBlazor.Extensions;
using MarkOfFlare.ViewModel;
using MarkOfFlare.Services;
using MarkOfFlare.Interfaces;
using MarkOfFlare.Models;

namespace MarkOfFlare
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMvvm();

            builder.Services.AddTransient<IFlareWizarClaimViewModel, FlareClaimViewModel>();
            builder.Services.AddTransient<IXrpKeyDeriviationViewModel, XrpKeyDeriviationViewModel>();
            builder.Services.AddTransient<IFlareSigningViewModel, FlareSigningViewModel>();

            builder.Services.AddSingleton<IMessenger, Messenger>();
            builder.Services.AddSingleton<IFlareSigner, FlareSigner>();

      await builder.Build().RunAsync();
        }
    }
}
