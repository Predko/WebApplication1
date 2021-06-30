using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.Extensions.Configuration;
using StorageDatabaseNameSpace;
using System.Text;
using System.Data;
using WebApplication1.Data;

namespace WebApplication1
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        private StorageDatabase storage;

        public Startup(IWebHostEnvironment env, IConfiguration conf)
        {
            Configuration = conf;
            Environment = env;

            storage = new(Configuration.GetConnectionString("CustomersDatabase"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();

            app.UseRouting();

            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(@"d:\")),

                RequestPath = new PathString("/disk_D")
            });

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\pages")),
                RequestPath = new PathString("/html")
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/info", async context =>
                {
                    await context.Response.WriteAsync(returnNamePlusValueString(nameof(Environment.ApplicationName), Environment.ApplicationName)
                        + returnNamePlusValueString(nameof(Environment.ContentRootPath), Environment.ContentRootPath)
                        + returnNamePlusValueString(nameof(Environment.ContentRootFileProvider), Environment.ContentRootFileProvider)
                        + returnNamePlusValueString(nameof(Environment.EnvironmentName), Environment.EnvironmentName)
                        + returnNamePlusValueString(nameof(Environment.WebRootPath), Environment.WebRootPath)
                        + returnNamePlusValueString(nameof(Environment.WebRootFileProvider), Environment.WebRootFileProvider));
                    ;
                });

                static string returnNamePlusValueString(string name, object var)
                {
                    return name + " = " + var + "\n";
                }
            });

            app.UseCustomers(storage);
        }
    }
}
