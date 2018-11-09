
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Fortune_Teller_UI.Services;
using Pivotal.Discovery.Client;
using Steeltoe.Security.DataProtection;
using Steeltoe.CloudFoundry.Connector.Redis;
using Steeltoe.CircuitBreaker.Hystrix;

namespace Fortune_Teller_UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            if (Environment.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddRedisConnectionMultiplexer(Configuration);
                services.AddDataProtection().PersistKeysToRedis().SetApplicationName("fortuneui");

                services.AddDistributedRedisCache(Configuration);
            }

            services.AddSession();
            services.AddMvc();

            services.AddTransient<IFortuneService, FortuneServiceClient>();

            services.Configure<FortuneServiceOptions>(Configuration.GetSection("fortuneService"));
            services.AddDiscoveryClient(Configuration);

            services.AddHystrixCommand<FortuneServiceCommand>("FortuneService", Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Fortunes/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Fortunes}/{action=Index}/{id?}");
            });

            app.UseDiscoveryClient();
        }
    }
}
