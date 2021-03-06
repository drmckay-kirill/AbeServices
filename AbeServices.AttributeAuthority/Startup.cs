﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AbeServices.AttributeAuthority.Models;
using AbeServices.AttributeAuthority.Services;
using AbeServices.Common.Protocols;
using AbeServices.Common.Helpers;

namespace AbeServices.AttributeAuthority
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
            services.Configure<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));
            services.Configure<MainSettings>(Configuration.GetSection("Main"));

            services.AddSingleton<ILoginService, LoginService>();
            services.AddSingleton<IPrivateKeyGenerator, PrivateKeyGenerator>();
            services.AddTransient<IDataSerializer, ProtobufDataSerializer>();
            services.AddTransient<IDataSymmetricEncryptor, DataSymmetricEncryption>();
            services.AddTransient<IKeyDistributionBuilder, KeyDistributionBuilder>();

            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
