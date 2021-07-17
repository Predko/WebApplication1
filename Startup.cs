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
using WebApplication1.Data.Middleware;

namespace WebApplication1
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public static readonly string BeginHtmlPages =
@"<!DOCTYPE html>
<html lang='ru' xml:lang='ru'>
<head>
    <meta charset='utf-8'/>
    <title>Main page</title>
    <link rel ='stylesheet' href='/Styles/Site.css'/>
</head>
<body>
    <header id='header_body'>
        <nav>
            <ul>
                <li class='menu-item'><a class='menu-item-link' href='/customers'> Клиенты </a></li>
                <li class='menu-item'><a class='menu-item-link' href='/customers/contracts'> Договоры </a></li>
                <li class='menu-item'><a class='menu-item-link' href='/customers/income'> Поступления </a></li>
                <li class='menu-item'><a class='menu-item-link' href='/customers/expenses'> Выплаты </a></li>
            </ul>
        </nav>
    </header>";

        public static readonly string EndHtmlPages =
@"    <footer id='footer_body'>
    </footer>
</body>";

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
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            //app.UseSession();

            app.UseDefaultFiles();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseCustomers(storage);

            app.UseContracts(storage);

            app.UseIncome(storage);

            app.UseExpenses(storage);
        }
    }
}
