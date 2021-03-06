﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AbeServices.IoTA.Settings;
using AbeServices.IoTA.Services;
using AbeServices.IoTA.Filters;
using AbeServices.Common.Extensions;
using AbeServices.Common.Models;

namespace AbeServices.IoTA
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
            services.Configure<DatabaseSettings>(Configuration.GetSection("Database"));
            services.Configure<MainSettings>(Configuration.GetSection("Main"));
            services.Configure<AbeSettings>(Configuration.GetSection("AbeSettings"));

            services.ConfigureProtocolBuilders();
            services.AddTransient<IEntityService, EntityService>();
            services.AddSingleton<IFiwareService, FiwareService>();
            services.AddTransient<AbeWriteAccessAuthorizationFilter>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
