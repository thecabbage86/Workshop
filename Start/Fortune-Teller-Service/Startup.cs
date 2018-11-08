using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Fortune_Teller_Service.Models;
using Pivotal.Discovery.Client;
using Microsoft.EntityFrameworkCore;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;

namespace Fortune_Teller_Service
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

            services.AddMvc();

            services.AddTransient<IFortuneRepository, FortuneRepository>();

            if(Environment.IsDevelopment())
            {
                services.AddEntityFrameworkInMemoryDatabase().AddDbContext<FortuneContext>(options => options.UseInMemoryDatabase("fortunes"));
            }
            else
            {
                services.AddDbContext<FortuneContext>(options => options.UseMySql(Configuration)); 
            }

            services.AddDiscoveryClient(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            SampleData.InitializeFortunesAsync(app.ApplicationServices).Wait();

            app.UseDiscoveryClient();
        }
    }
}
