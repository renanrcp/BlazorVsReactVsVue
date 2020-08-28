using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor
{
    public class Startup
    {
        public Startup(WebAssemblyHostConfiguration configuration, IWebAssemblyHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public WebAssemblyHostConfiguration Configuration { get; }

        public IWebAssemblyHostEnvironment HostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(HostEnvironment.BaseAddress)
            });
        }
    }
}