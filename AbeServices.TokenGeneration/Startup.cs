using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AbeServices.TokenGeneration.Settings;
using AbeServices.TokenGeneration.Services;
using AbeServices.Common.Helpers;
using AbeServices.Common.Protocols;
using AbeServices.Common.Models;

namespace AbeServices.TokenGeneration
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MainSettings>(Configuration.GetSection("Main"));
            services.Configure<AbeSettings>(Configuration.GetSection("AbeSettings"));

            services.AddTransient<IKeyDistributionBuilder, KeyDistributionBuilder>();
            services.AddTransient<IAbeDecorator, AbeDecorator>();
            services.AddTransient<IDataSerializer, ProtobufDataSerializer>();
            services.AddTransient<IAbeAuthBuilder, AbeAuthBuilder>();
            services.AddSingleton<ITokensService, TokensService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
