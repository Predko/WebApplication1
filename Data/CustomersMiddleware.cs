using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public class CustomersMiddleware
    {
        private RequestDelegate Next { get; }

        private readonly IStorageDatabase storage;

        public CustomersMiddleware(RequestDelegate next, StorageDatabase storage)
        {
            Next = next;

            this.storage = storage;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();

            var customer = context.Request.Query["customer"];

            if (path == "/customers")
            {
                if (string.IsNullOrWhiteSpace(customer) == false && int.TryParse(customer, out int id) == true)
                {
                    await ShowCustomer(context, id);

                    return;
                }
            }
            else
            {
                await Next.Invoke(context);

                return;
            }

            StringBuilder response = new(string.Format(Startup.BeginHtmlPages,
                                                "<link rel = \"stylesheet\" href= \"/Styles/Customers.css\" />") +
@"
    <main>
        <div id=""titleTable""><h1 id=""h1ListCustomers"">Список клиентов</h1></div>

        <div style=""overflow-y:auto"" id=""divTable"">
        <table>
            <thead>
                <tr>
                    <th data-sort-order=""ascending"">УНП</th>
                    <th data-sort-order=""ascending"">Название организации</th>
                </tr>
            </thead>
            <tbody>");

            DataView dataViewTable;

            try
            {
                dataViewTable = storage["Customers"].DefaultView;
            }
            catch(SqliteException ex)
            {
                response.Clear().Append(ex.Message);

                await context.Response.WriteAsync(response.ToString());

                return;
            }

            dataViewTable.Sort = "NameCompany";

            foreach (DataRowView row in dataViewTable)
            {
                string unp = row["UNP"] as string;

                response.Append(string.Format("<tr id=\"{0}\"><td>{1}</td><td>{2}</td></tr>", 
                                                row["Id"].ToString(), unp ?? "", row["NameCompany"]));
            }

            response.Append(@"
            </tbody>
        </table>
        </div>
    </main>" +

Startup.EndHtmlPages +

@"<script src=""js\sorttable.js"" type=""text/javascript""></script>
<script src=""js\scripts.js"" type=""text/javascript""></script>
<html>");

            await context.Response.WriteAsync(response.ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ShowCustomer(HttpContext context, int id)
        {
            DataRow row = storage["Customers", $"SELECT * FROM Customers WHERE Id='{id}'"].Rows[0];

            StringBuilder response = new(string.Format(Startup.BeginHtmlPages,
                                                       "<link rel = \"stylesheet\" href= \"/Styles/Customers.css\" />"));

            if (row == null)
            {
                response.Append(
@"
    <main>
        < h1 color=""red"">Указанный клиент не найден<h1>
    </main>" +
        
        Startup.EndHtmlPages + "</html>");
            }
            else
            {
                response.Append("" +
    $"<main><h1>{row["NameCompany"]}</h1>" +
        "<table>" +
            "<tbody>" + 
                "<table>" +
                    $"<tr><td>УНП</td><td>{row["UNP"]}</td></tr>" +
                    $"<tr><td>Расчётный счёт</td><td>{row["account"]}</td></tr>" +
                    $"<tr><td>Город</td><td>{row["city"]}</td></tr>" +
                    $"<tr><td>Дополнительный расчётный счёт</td><td>{row["account1"]}</td></tr>" +
                    $"<tr><td>Область</td><td>{row["region"]}</td></tr>" +
                    $"<tr><td>Номер телефона</td><td>{row["phoneNumber"]}</td></tr>" +
                    $"<tr><td>Факс</td><td>{row["fax"]}</td></tr>" +
                    $"<tr><td>Электронная почта</td><td>{row["mail"]}</td></tr>" +
                    $"<tr><td>Файл с дополнительной информацией</td><td>{row["file"]}</td></tr>" +
                    "</table>" +
                    "</main>" +
                    Startup.EndHtmlPages + "</html>");
            }

            await context.Response.WriteAsync(response.ToString());
        }

    }
}
