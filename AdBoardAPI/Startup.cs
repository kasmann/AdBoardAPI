using System.IO;
using AdBoardAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using AdBoardAPI.ImageResizer;

namespace AdBoardAPI
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
            var connection = Configuration.GetConnectionString("AdBoardContext");
            services.AddDbContext<AdBoardContext>(options => options.UseSqlServer(connection));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AdBoardAPI", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "AdBoardAPI.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddImageResizer();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseImageResizer();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdBoardAPI v1");
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, Configuration.GetSection("AppConfiguration")["staticFilesRoot"])),
                    RequestPath = new PathString(Configuration.GetSection("AppConfiguration")["staticFilesRoot"])
                    
                });
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
