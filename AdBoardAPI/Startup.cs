using AdBoardAPI.CustomCache.CustomCacheController;
using AdBoardAPI.ImageResizer;
using AdBoardAPI.Options;
using AdBoardAPI.Options.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.IO;

namespace AdBoardAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppConfiguration>(Configuration.GetSection(nameof(AppConfiguration)));
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<AppConfiguration>>().Value);

            services.AddSingleton<IValidatable>(resolver =>
                resolver.GetRequiredService<IOptions<AppConfiguration>>().Value);

            services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();

            var connection = Configuration.GetConnectionString("AdBoardContext");
            services.AddDbContext<AdBoardContext>(options => options.UseSqlServer(connection));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AdBoardAPI", Version = "v1" });
                var filePath = Path.Join(System.AppContext.BaseDirectory, "AdBoardAPI.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddSingleton<ICustomImageCacheController, PhysicalImageCacheController>();

            var factory = LoggerFactory.Create(builder => builder.AddConsole());
            services.AddImageResizer(factory);

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<AppConfiguration> options)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(appBuilder => appBuilder.UseGlobalExceptionHandler());

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
                    FileProvider = new PhysicalFileProvider(Path.Join(env.WebRootPath, options.Value.SystemOptions.StaticFilesRoot)),
                    RequestPath = new PathString(options.Value.SystemOptions.StaticFilesRoot)
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
