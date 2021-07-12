using Microsoft.AspNetCore.Http;
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
    public class CustomersMiddleware : AbstractCustomersMiddlware
    {
        protected override string TableName { get => "Customers"; }

        protected override string EntityName { get => "customer"; }

        protected override string ListEntities { get => "/customers"; }

        protected override string EditEntity { get => "/customers/edit"; }

        protected override string DeleteEntity { get => "/customers/delete"; }

        protected override string NewEntity { get => "/customers/new"; }

        protected override string ContextMenu { get; }

        public CustomersMiddleware(RequestDelegate next, StorageDatabase storage) : base(next, storage)
        {
            ContextMenu = ContextMenuString.GetBuilder()
                .Append("edit", "Просмотр и редактирование")
                .Append("contracts", "Список договоров")
                .Append("income", "Оплата договоров")
                .Append("new", "Добавить нового клиента")
                .Append("delete", "Удалить клиента")
                .GetContextMenuString();
        }

        /// <summary>
        /// Отображает список клиентов.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task<bool> ShowListOfEntities(HttpContext context, int customerId)
        {
            StringBuilder response = new(string.Format(Startup.BeginHtmlPages,
                                            "<link rel = 'stylesheet' href= '/Styles/Customers.css' />"));
            response.Append("<main>")
                    .Append("<div id='titleTable'><h1 id='h1ListCustomers'>Список клиентов</h1></div>")
                    .Append("<div style='overflow-y:auto' id='divTable'>")
                    .Append("<table id='list-customers' data-parameter='customer'><thead><tr>")
                    .Append("<th data-sort-order='ascending'>УНП</th>")
                    .Append("<th data-sort-order='ascending'>Название организации</th>")
                    .Append("</tr></thead><tbody>");

            DataView dataViewTable;

            try
            {
                dataViewTable = Storage[TableName].DefaultView;
            }
            catch (SqliteException ex)
            {
                response.Clear().Append(ex.Message);

                await context.Response.WriteAsync(response.ToString());

                return true;
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
                    .Append(ContextMenu)
                    .Append(Startup.EndHtmlPages)
                    .Append("<script src='js\\HandlerTS.js'></script><html>");   // <script src=""js\scripts.js"" type=""text/javascript""></script>

            await context.Response.WriteAsync(response.ToString());

            return false;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override async Task ShowEditEntity(HttpContext context, int customerId, int? entityId)
        {
            DataRow row = Storage["Customers", $"SELECT * FROM Customers WHERE Id='{customerId}'"].Rows[0];

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
                        .Append($"<form method='post' action='{EditEntity}/submit'>")
                        .Append($"<input type='hidden' name='Id' value='{row["Id"]}'/>")

                        .Append("<table><tbody>")
                        .Append($"<tr><td><label>Название организации</label></td>")
                        .Append($"<td><input name='NameCompany' value='{row["NameCompany"]}'/></td></tr>")

                        .Append($"<tr><td><label>УНП</label></td>")
                        .Append($"<td><input name='UNP' value='{row["UNP"]}'/></td></tr>")

                        .Append($"<tr><td><label>Расчётный счёт</label></td>")
                        .Append($"<td><input name='account' value='{row["account"]}'/></td></tr>")

                        .Append($"<tr><td><label>Город</label></td>")
                        .Append($"<td><input name='city' value='{row["city"]}'/></td></tr>")

                        .Append($"<tr><td><label>Дополнительный расчётный счёт</label></td>")
                        .Append($"<td><input name='account1' value='{row["account1"]}'/></td></tr>")

                        .Append($"<tr><td><label>Район</label></td>")
                        .Append($"<td><input name='region' value='{row["region"]}'/></td></tr>")

                        .Append($"<tr><td><label>Номер телефона</label></td>")
                        .Append($"<td><input name='phoneNumber' value='{row["phoneNumber"]}'/></td></tr>")

                        .Append($"<tr><td><label>Номер факса</label></td>")
                        .Append($"<td><input name='fax' value='{row["fax"]}'/></td></tr>")

                        .Append($"<tr><td><label>Электронная почта</label></td>")
                        .Append($"<td><input name='mail' value='{row["mail"]}'/></td></tr>")

                        .Append($"<tr><td><label>Файл с дополнительной информацией</label></td>")
                        .Append($"<td><input name='file' value='{row["file"]}'/></td></tr>")
                        .Append("</tbody></table>")

                        .Append("<p><input type='submit' value='Отправить'>")
                        .Append("<input type='reset' value='Очистить'></p>")

                        .Append("</form>")
                        .Append("</main>")
                        .Append(Startup.EndHtmlPages + "</html>");
            }

            await context.Response.WriteAsync(response.ToString());
        }

        //protected override async Task ShowDeleteEntity(HttpContext context, int customerId, int? incomeId)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override async Task ShowNewEntity(HttpContext context, int customerId)
        //{
        //    throw new NotImplementedException();
        //}

        protected override async Task ProcessRequest(HttpContext context, int customerId)
        {



            await context.Response.WriteAsync("Данные обновлены для киента с id = " + customerId);
        }
    }
}
