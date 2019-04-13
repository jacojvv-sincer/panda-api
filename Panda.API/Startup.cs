using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using Panda.API.Data;
using Panda.API.Middleware;

namespace Panda.API
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
            // IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme
            ).AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = $"https://login.microsoftonline.com/tfp/{Configuration["AzureAdB2C:Tenant"]}/{Configuration["AzureAdB2C:Policy"]}/v2.0/";
                jwtOptions.Audience = Configuration["AzureAdB2C:ClientId"];
                jwtOptions.TokenValidationParameters.ValidIssuer = $"https://{Configuration["AzureAdB2C:IssuerDomain"]}/{Configuration["AzureAdB2C:IssuerId"]}/v2.0/";
                jwtOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = AuthenticationFailed
                };
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddHttpContextAccessor();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors(builder =>
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );

            app.UseAuthentication();
            app.UseSetApplicationUser();

            app.UseMvc();
        }

        private Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // For debugging purposes only!
            var s = $"AuthenticationFailed: {arg.Exception.Message}";
            arg.Response.ContentLength = s.Length;
            arg.Response.Body.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            return Task.FromResult(0);
        }
    }
}
