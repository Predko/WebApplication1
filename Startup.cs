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

        public static readonly string BeginHtmlPages =
@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8""/>
    <title>Main page</title>
    <link rel =""stylesheet"" href=""/Styles/Site.css""/>
    {0}
</head>
<body onresize=""resizebody()"" onload=""resizebody()"">
    <header id=""header_body"">
        <nav>
            <ul>
                <li class=""menu-item""><a class=""menu-item-link"" href=""/index.html""> Pages </a></li>
                <li class=""menu-item""><a class=""menu-item-link"" href=""/customers""> Customers </a></li>
                <li class=""menu-item""><a class=""menu-item-link"" href=""/info""> Info </a></li>
                <li class=""menu-item""><a class=""menu-item-link"" href=""/disk_D""> disk D</a></li>
            </ul>
        </nav>
    </header>";

        public static readonly string EndHtmlPages =
@"    <footer id=""footer_body"">
    </footer>
</body>
";

        private StorageDatabase storage;

        public Startup(IWebHostEnvironment env, IConfiguration conf)
        {
            Configuration = conf;
            Environment = env;

            storage = new SqliteDatabaseStore(Configuration.GetConnectionString("CustomersSqliteDatabase"));
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
