using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            services.AddSwaggerGen(a=>
            {
                a.SwaggerDoc("v1", new Info {Title="SayYes" });
                a.AddSecurityDefinition("Bearer", new ApiKeyScheme() { In = "header", Description = "Please insert JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "YesNoApi.xml");
                a.IncludeXmlComments(xmlPath);
            });
            services.AddMvc();
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

            app.UseDefaultFiles().UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                RequestPath = new PathString("/static")
            });


            app.UseSwagger();
            app.UseSwaggerUI(a=>
            {
                a.SwaggerEndpoint("/swagger/v1/swagger.json", "SayYes Documentation");
            });


            app.UseMvc();
        }
    }
}
