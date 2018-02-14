using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace YesNoApi
{
    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        const string _adb2cSchemeName = "adB2c";
        const string _googleSchemeName = "GOOGLE";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:YesNoApi.Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <remarks>This method gets called by the runtime. Use this method to add services to the container.</remarks>
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(b =>
            {
                b.DefaultAuthenticateScheme = _adb2cSchemeName;
                b.DefaultChallengeScheme = _adb2cSchemeName;
            }).AddJwtBearer(_adb2cSchemeName, a =>
            {
                a.Audience = "96e7efbe-a021-4e1c-a6fa-38a543183c7b";
                a.Authority = "https://login.microsoftonline.com/tfp/c66ea553-07d4-47bf-b0d2-689e2b6e7329/b2c_1_yesnopolicy/v2.0/";
            }).AddJwtBearer(_googleSchemeName, b=>
            {
                b.Authority = "https://accounts.google.com";
                b.Audience = "114985050436-jrp914eou6kp12665mg8k0nloco6tc13.apps.googleusercontent.com";
                b.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = "accounts.google.com",
                };
            }); 

            services.AddSwaggerGen(a=>
            {
                a.SwaggerDoc("v1", new Info {Title="SayYes" });
                a.AddSecurityDefinition("Bearer", new ApiKeyScheme() { In = "header", Description = "Please insert JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "YesNoApi.xml");
                a.IncludeXmlComments(xmlPath);
            });
            services.AddMvc(c=>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                c.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        /// <summary>
        /// Configure the specified app and env.
        /// </summary>
        /// <returns>The configure.</returns>
        /// <param name="app">App.</param>
        /// <param name="env">Env.</param>
        /// <remarks>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</remarks>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = "/static",
                EnableDirectoryBrowsing = true
            });


            app.UseSwagger();
            app.UseSwaggerUI(a=>
            {
                a.SwaggerEndpoint("/swagger/v1/swagger.json", "SayYes Documentation");
            });
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                var principal = new ClaimsPrincipal();

                var result1 = await context.AuthenticateAsync(_adb2cSchemeName);
                if (result1?.Principal != null)
                {
                    foreach (ClaimsIdentity ci in result1.Principal.Identities)
                    {
                        ci.AddClaim(new Claim(ClaimTypes.Role,_adb2cSchemeName));
                    }
                    principal.AddIdentities(result1.Principal.Identities);
                }
                var result2 = await context.AuthenticateAsync(_googleSchemeName);
                if (result2?.Principal != null)
                {
                    foreach (ClaimsIdentity ci in result2.Principal.Identities)
                    {
                        ci.AddClaim(new Claim(ClaimTypes.Role, _googleSchemeName));
                    }
                    principal.AddIdentities(result2.Principal.Identities);
                }


                context.User = principal;

                await next();
            });

            app.UseMvc();
        }

    }
}
