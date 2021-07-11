﻿using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Реализация контекстного меню взята из: https://habr.com/ru/post/258167
 */


namespace WebApplication1.Data
{
    public class CustomersMiddleware
    {
        private RequestDelegate Next { get; }

        private readonly StorageDatabase storage;

        private readonly string contextMenu = "" +
@"<nav class='context-menu' id='context-menu'>
  <ul class='context-menu__items'>
    <li class='context-menu__item'>
      <a href = '#' class='context-menu__link' data-action='contracts'>
        <i class='fa fa-eye'></i>Договоры
      </a>
    </li>
    <li class='context-menu__item'>
      <a href = '#' class='context-menu__link' data-action='income'>
        <i class='fa fa-edit'></i>Оплата договоров
      </a>
    </li>
    <li class='context-menu__item'>
      <a href = '#' class='context-menu__link' data-action='edit'>
        <i class='fa fa-times'></i>Просмотр и редактирование
      </a>
    </li>
    <li class='context-menu__item'>
      <a href = '#' class='context-menu__link' data-action='delete'>
        <i class='fa fa-times'></i>Удалить
      </a>
    </li>
  </ul>
</nav>";

        public CustomersMiddleware(RequestDelegate next, StorageDatabase storage)
        {
            Next = next;

            this.storage = storage;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();
            
            string customer_Id = context.Request.Query["customer"];
            
            if (string.IsNullOrWhiteSpace(customer_Id) == true || int.TryParse(customer_Id, out int customerId) == false)
            {
                customerId = 0;
            }

            switch (path)
            {
                case "/customers":
                    await ShowListCustomers(context);
                    break;

                case "/customers/edit":
                    if (customerId != 0)
                    {
                        await ShowEditCustomer(context, customerId);
                    }
                    break;

                case "/customers/delete":
                    if (customerId != 0)
                    {
                        await ShowDeleteCustomer(context, customerId);
                    }

                    break;

                case "/customers/new":
                    if (customerId != 0)
                    {
                        await ShowNewCustomer(context);
                    }

                    break;

                default:

                    await Next.Invoke(context);
                    break;
            }
        }

        private Task ShowNewCustomer(HttpContext context)
        {
            throw new NotImplementedException();
        }

        private Task ShowDeleteCustomer(HttpContext context, int customerId)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Отображает список клиентов.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ShowListCustomers(HttpContext context)
        {


            StringBuilder response = new(string.Format(Startup.BeginHtmlPages,
                                            "<link rel = 'stylesheet' href= '/Styles/Customers.css' />"));
            response.Append("<main>")
                    .Append("<div id='titleTable'><h1 id='h1ListCustomers'>Список клиентов</h1></div>")
                    .Append("<div style='overflow-y:auto' id='divTable'>")
                    .Append("<table id='list-customers'><thead><tr>")
                    .Append("<th data-sort-order='ascending'>УНП</th>")
                    .Append("<th data-sort-order='ascending'>Название организации</th>")
                    .Append("</tr></thead><tbody>");

            DataView dataViewTable;

            try
            {
                dataViewTable = storage["Customers"].DefaultView;
            }
            catch (SqliteException ex)
            {
                response.Clear().Append(ex.Message);

                await context.Response.WriteAsync(response.ToString());

                return;
            }

            dataViewTable.Sort = "NameCompany";

            foreach (DataRowView row in dataViewTable)
            {
                string unp = row["UNP"] as string;

                response.Append(string.Format("<tr class='task' id='{0}'><td>{1}</td><td>{2}</td></tr>",
                                                row["Id"].ToString(),
                                                unp ?? "",
                                                row["NameCompany"]));
            }

            response.Append("</tbody></table></div></main>")
                    .Append(contextMenu)
                    .Append(Startup.EndHtmlPages)
                    .Append("<script src='js\\HandlerTS.js'></script><html>");   // <script src=""js\scripts.js"" type=""text/javascript""></script>

            await context.Response.WriteAsync(response.ToString());
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ShowEditCustomer(HttpContext context, int id)
        {
            DataRow row = storage["Customers", $"SELECT * FROM Customers WHERE Id='{id}'"].Rows[0];

            StringBuilder response = new(string.Format(Startup.BeginHtmlPages,
                                                       "<link rel = \"stylesheet\" href= \"/Styles/Customers.css\" />"));

            if (row == null)
            {
                response.Append(@"<main><h1 color=""red"">Указанный клиент не найден</h1></main>")
                        .Append(Startup.EndHtmlPages + "</html>");
            }
            else
            {
                response.Append($"<main><h1>{row["NameCompany"]}</h1>")
                        .Append("<table>")
                        .Append("<tbody>")
                        .Append($"<tr><td>УНП</td><td>{row["UNP"]}</td></tr>")
                        .Append($"<tr><td>Расчётный счёт</td><td>{row["account"]}</td></tr>")
                        .Append($"<tr><td>Город</td><td>{row["city"]}</td></tr>")
                        .Append($"<tr><td>Дополнительный расчётный счёт</td><td>{row["account1"]}</td></tr>")
                        .Append($"<tr><td>Область</td><td>{row["region"]}</td></tr>")
                        .Append($"<tr><td>Номер телефона</td><td>{row["phoneNumber"]}</td></tr>")
                        .Append($"<tr><td>Факс</td><td>{row["fax"]}</td></tr>")
                        .Append($"<tr><td>Электронная почта</td><td>{row["mail"]}</td></tr>")
                        .Append($"<tr><td>Файл с дополнительной информацией</td><td>{row["file"]}</td></tr>")
                        .Append("</table>")
                        .Append("</main>")
                        .Append(Startup.EndHtmlPages + "</html>");
            }

            await context.Response.WriteAsync(response.ToString());
        }

    }
}
