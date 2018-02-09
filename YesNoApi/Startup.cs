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
        const string _schemeName = "adB2c";
        const string _googleScheme = "GOOGLE";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:YesNoApi.Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
                b.DefaultAuthenticateScheme = _schemeName;
                b.DefaultChallengeScheme = _schemeName;
            }).AddJwtBearer(_schemeName, a =>
            {
                a.Audience = "96e7efbe-a021-4e1c-a6fa-38a543183c7b";
                a.Authority = "https://login.microsoftonline.com/tfp/c66ea553-07d4-47bf-b0d2-689e2b6e7329/b2c_1_yesnopolicy/v2.0/";
            }).AddJwtBearer(_googleScheme, b=>
            {
                b.Authority = "https://accounts.google.com";
                //b.MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration";
                b.Audience = "114985050436-jrp914eou6kp12665mg8k0nloco6tc13.apps.googleusercontent.com";
                //b.RequireHttpsMetadata = false;
                b.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = "accounts.google.com",
                //    ValidAudience = "114985050436-jrp914eou6kp12665mg8k0nloco6tc13.apps.googleusercontent.com",
                //    ValidateAudience = true,
                //    ValidateIssuer = true,
                //    RequireSignedTokens = true,
                //    ValidateIssuerSigningKey = true
                    
                };
            //}).AddOpenIdConnect(_googleScheme, t=>
            //{
            //    t.Authority = "https://accounts.google.com";
            //    t.ClientId = "114985050436-jrp914eou6kp12665mg8k0nloco6tc13.apps.googleusercontent.com";
            }); 
            //.AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = "114985050436-jrp914eou6kp12665mg8k0nloco6tc13.apps.googleusercontent.com";// "114985050436-jrp914eou6kp12665mg8k0nloco6tc13.apps.googleusercontent.com";
            //    googleOptions.ClientSecret = "D6IVXbXGEq6ndAMgcBji-lKS";// "D6IVXbXGEq6ndAMgcBji-lKS";
            //});

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

                var result1 = await context.AuthenticateAsync(_schemeName);
                if (result1?.Principal != null)
                {
                    foreach (ClaimsIdentity ci in result1.Principal.Identities)
                    {
                        ci.AddClaim(new Claim(ClaimTypes.Role, "ADB2C"));
                    }
                    principal.AddIdentities(result1.Principal.Identities);
                }
                var result2 = await context.AuthenticateAsync(_googleScheme);
                if (result2?.Principal != null)
                {
                    foreach (ClaimsIdentity ci in result2.Principal.Identities)
                    {
                        ci.AddClaim(new Claim(ClaimTypes.Role, _googleScheme));
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
