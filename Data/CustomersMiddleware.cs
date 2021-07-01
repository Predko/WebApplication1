using Microsoft.AspNetCore.Http;
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

        private readonly StorageDatabase storage;

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

            StringBuilder response = new(string.Format(Startup.BeginHtmlPages, "<link rel = \"stylesheet\" href= \"/Styles/Customers.css\" />") +
@"
    <main>
        <h1>Список клиентов</h1>
        <table>
            <thead>
                <tr><th>УНП</th><th>Название организации</th></tr>
            </thead>
            <tbody>");

            foreach (DataRow row in storage["Customers"].Rows)
            {
                string unp = row["UNP"] as string;

                response.Append(string.Format("<tr id=\"{0}\"><td>{1}</td><td>{2}</td></tr>", row["Id"].ToString(), unp ?? "", row["NameCompany"]));
            }

            response.Append(
@"            </tbody>
        </table>
    </main>" +

Startup.EndHtmlPages + 

@"
<script>
    document.querySelector('table').onclick = (event) => {
        var cell = event.target;
        if (cell.tagName.toLowerCase() != 'td')
            return;
        var i = cell.parentNode.rowIndex;
        var j = cell.cellIndex;
        var currentTr = cell.parentNode;
        window.location.href = ""customers?customer="" + currentTr.id;
    }
</script> 
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
            DataRow rowId = null;

            foreach (DataRow row in storage["Customers"].Rows)
            {
                if ((int)row["Id"] == id)
                {
                    rowId = row;

                    break;
                }
            }

            StringBuilder response = new(string.Format(Startup.BeginHtmlPages,
                                                       "<link rel = \"stylesheet\" href= \"/Styles/Customers.css\" />"));

            if (rowId == null)
            {
                response.Append(
@"
    <main>
        < h1 color=""red"">Указанный клиент не найден<h1>
    </main>" +
        
        Startup.EndHtmlPages +
@"
<html>");
            }
            else
            {
                response.Append(@"
    <main>
        <table>
            <tbody>" + 
                $"<table><h1>{rowId["NameCompany"]}</h1>" +
                    $"<tr><td>УНП</td><td>{rowId["UNP"]}</td></tr>" +
                    $"<tr><td>Расчётный счёт</td><td>{rowId["account"]}</td></tr>" +
                    $"<tr><td>Город</td><td>{rowId["city"]}</td></tr>" +
                    $"<tr><td>Дополнительный расчётный счёт</td><td>{rowId["account1"]}</td></tr>" +
                    $"<tr><td>Область</td><td>{rowId["region"]}</td></tr>" +
                    $"<tr><td>Номер телефона</td><td>{rowId["phoneNumber"]}</td></tr>" +
                    $"<tr><td>Факс</td><td>{rowId["fax"]}</td></tr>" +
                    $"<tr><td>Электронная почта</td><td>{rowId["mail"]}</td></tr>" +
                    $"<tr><td>Файл с дополнительной информацией</td><td>{rowId["file"]}</td></tr>" +
                    "</table>" +
                    "</main>" +
                    Startup.EndHtmlPages +
                    "<html>");
            }

            await context.Response.WriteAsync(response.ToString());
        }

    }
}
