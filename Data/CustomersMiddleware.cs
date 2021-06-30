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

            StringBuilder response = new("<!DOCTYPE html>" +
                "\n<html>" +
                "\n<head>" +
                "\n<meta charset=\"utf-8\"/>" +
                "\n<style>" +
                "\nth,td {" +
                "\npadding</td><td>3px;" +
                "\n}" +
                "\ntr:nth-child(even) {" +
                "\nbackground</td><td>#ddd;" +
                "\n}" +
                "\ntr</td><td>nth - child(odd) {" +
                "\nbackground</td><td>#eee;" +
                "\n}" +
                "\n</style>" +
                "\n</head>" +
                "\n<body>" +
                "\n<h1>Список клиентов</h1>" +
                "\n<table>" +
                "\n<thead>" +
                "\n<tr><th>УНП</th><th>Название организации</th></tr>" +
                "\n</thead>" +
                "\n<tbody>");

            foreach (DataRow row in storage["Customers"].Rows)
            {
                string unp = row["UNP"] as string;

                response.Append(string.Format("\n<tr id=\"{0}\"><td>{1}</td><td>{2}</td></tr>", row["Id"].ToString(), unp ?? "", row["NameCompany"]));
            }

            response.Append("" +
                "\n</tbody>" +
                "\n</table>" +
                "</script>" +
                "\n</body>" +
                "<script>" +
                "document.querySelector('table').onclick = (event) => {" +
                "var cell = event.target;" +
                "if (cell.tagName.toLowerCase() != 'td')" +
                    "return;" +
                "var i = cell.parentNode.rowIndex;" +
                "var j = cell.cellIndex;" +
                "var currentTr = cell.parentNode;" +
                "window.location.href = \"customers?customer=\" + currentTr.id" +
                "}" +
                "</script> " +
                "\n<html>");

            await context.Response.WriteAsync(response.ToString());
        }

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

            StringBuilder response = new("" +
                "<!DOCTYPE html>" +
                "\n<html>" +
                "\n<head>" +
                "\n<meta charset=\"utf-8\"/>" +
                "\n</head>" +
                "\n<body>");

            if (rowId == null)
            {
                response.Append("\n<h1 color=\"red\">Указанный клиент не найден<h1>" +
                    "\n</body>" +
                    "\n<html>");
            }
            else
            {
                response.Append("" +
                    $"\n<table><h1>{rowId["NameCompany"]}</h1>" +
                    $"\n<tr><td>УНП</td><td>{rowId["UNP"]}</td></tr>" +
                    $"\n<tr><td>Расчётный счёт</td><td>{rowId["account"]}</td></tr>" +
                    $"\n<tr><td>Город</td><td>{rowId["city"]}</td></tr>" +
                    $"\n<tr><td>Дополнительный расчётный счёт</td><td>{rowId["account1"]}</td></tr>" +
                    $"\n<tr><td>Область</td><td>{rowId["region"]}</td></tr>" +
                    $"\n<tr><td>Номер телефона</td><td>{rowId["phoneNumber"]}</td></tr>" +
                    $"\n<tr><td>Факс</td><td>{rowId["fax"]}</td></tr>" +
                    $"\n<tr><td>Электронная почта</td><td>{rowId["mail"]}</td></tr>" +
                    $"\n<tr><td>Файл с дополнительной информацией</td><td>{rowId["file"]}</td></tr>" +
                    "\n</table>" +
                    "\n</body>" +
                    "\n<html>");
            }

            await context.Response.WriteAsync(response.ToString());
        }

    }
}
