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


namespace WebApplication1.Data.Middleware.Customers
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

        private readonly string[] columnNames =
        {
            "Идентификатор",
            "Название организации",
            "Расчётный счёт",
            "Город",
            "Дополнительный расчётный счёт",
            "Район",
            "Номер телефона",
            "Номер факса",
            "Электронная почта",
            "Файл с дополнительной информацией",
            "УНП"
        };


        public CustomersMiddleware(RequestDelegate next, StorageDatabase storage) : base(next, storage)
        {
            ContextMenu = ContextMenuString.Builder()
                .Append("edit", "Просмотр и редактирование")
                .Append("contracts", "Список договоров")
                .Append("income", "Оплата договоров")
                .Append("new", "Добавить нового клиента")
                .Append("delete", "Удалить клиента")
                .Build();
        }

        /// <summary>
        /// Отображает список клиентов.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>false if an error is occured, otherwise - true</returns>
        protected override async Task<bool> ShowListOfEntities(HttpContext context, int p1, int p2)
        {
            DataView dataViewTable;

            try
            {
                dataViewTable = Storage[TableName].DefaultView;
            }
            catch (SqliteException ex)
            {
                await context.Response.WriteAsync(ex.Message);

                return true;
            }

            dataViewTable.Sort = dataViewTable.Table.Columns[1].ColumnName;

            StringBuilder tbody = new("<tbody>");

            const int columnsNumber = 2;
            int[] maxlength = new int[columnsNumber];

            foreach (DataRowView row in dataViewTable)
            {
                string unp = row["UNP"] as string;
                string nameCompany = ((string)row["NameCompany"]);
                
                maxlength[0] = unp.Length > maxlength[0] ? unp.Length : maxlength[0];
                maxlength[1] = nameCompany.Length > maxlength[1] ? nameCompany.Length : maxlength[1];

                tbody.Append(string.Format("<tr class='task' id='{0}'><td>{1}</td><td>{2}</td></tr>",
                                                row["Id"].ToString(),
                                                unp ?? "",
                                                nameCompany));
            }
            
            StringBuilder response = new StringBuilder($"<div id='titleTable'><h1 id='h1ListEntities'>Список клиентов</h1></div>");

            string containerHeight = context.Request.Cookies["containerHeight"];

            if (containerHeight != null)
            {
                response.Append($"<div style='overflow-y:auto' id='divTable' style='height:{containerHeight}px'>");
            }
            else
            {
                response.Append($"<div style='overflow-y:auto' id='divTable'>");
            }

            response.Append($"<table id='list-entities' data-parameter='{EntityName}'><thead><tr>")
                    .Append($"<th width={maxlength[0]}ex data-sort-order='ascending'>УНП</th>")
                    .Append($"<th data-sort-order='ascending'>Название организации</th>")   //  width={columnsWidth[1]}%
                    .Append("</tr></thead>")
                    .Append(tbody).Append("</tbody>")
                    .Append("</table></div>")
                    .Append(ContextMenu);

            await context.Response.WriteAsync(response.ToString());

            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns>False if an error is occured, otherwise - true</returns>
        protected override async Task<bool> ShowEditEntity(HttpContext context, int customerId, int entityId)
        {
            DataTable dataTable = Storage[TableName, $"SELECT * FROM Customers WHERE Id='{customerId}'"];
            DataRow row = dataTable.Rows[0];

            StringBuilder response = new(Startup.BeginHtmlPages);

            if (row == null)
            {
                response.Append(@"<main><h1 color=""red"">Указанный клиент не найден</h1></main>")
                        .Append(Startup.EndHtmlPages + "</html>");
            }
            else
            {
                response.Append($"<main id='main-container'><h1>Подробные данные клиента</h1>")
                        .Append($"<form id='formId'>")
                        .Append($"<input type='hidden' name='{CustomerEntityName}Id' value='{row["Id"]}'/>")

                        .Append("<table><tbody>");

                var columns = dataTable.Columns;

                for (int i = 1; i != row.ItemArray.Count(); i++)
                {
                    response.Append($"<tr><td><label>{columnNames[i]}</label></td>")
                            .Append($"<td><input name='{columns[i].ColumnName}' value='{row[i]}'/></td></tr>");
                }

                response.Append("</tbody></table>")

                        .Append("<p><input type='submit' value='Отправить'>")
                        .Append("<input type='reset' value='Очистить'></p>")

                        .Append("</form>")
                        .Append("</main>")
                        .Append(Startup.EndHtmlPages ).Append("<script src='/js/SubmitForm.js'></script><html>");
            }

            await context.Response.WriteAsync(response.ToString());

            return true;
        }

        //protected override async Task<bool> ShowDeleteEntity(HttpContext context, int customerId, int incomeId)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override async Task<bool> ShowNewEntity(HttpContext context, int customerId, int p2)
        //{
        //    throw new NotImplementedException();
        //}

        protected override async Task<bool> ProcessFormRequest(HttpContext context, int customerId, int p2)
        {
            if (customerId == -1)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                
                await context.Response.WriteAsync("Ошибка запроса. Отсутствует Id клиента");

                return true;
            }

            var formValues = context.Request.Form.Select(e => e.Value).ToArray();

            DataTable dataTable = Storage["Customers", $"SELECT * FROM Customers WHERE Id='{customerId}'"];
            DataRow row = dataTable.Rows[0];

            row.BeginEdit();

            for (int i = 1; i != row.ItemArray.Count(); i++)
            {
                Type t = row.ItemArray[i].GetType();

                row[i] = GetRowValueFromString(row.ItemArray[i].GetType(), formValues[i]);
            }

            row.EndEdit();

            Storage.UpdateDataTable("Customers");

            context.Response.StatusCode = StatusCodes.Status202Accepted;
            context.Response.ContentType = "application/text; charset=UTF-8";

            await context.Response.WriteAsync($"Данные для клиента с id = {customerId} обновлены успешно");

            return true;
        }

    private object GetRowValueFromString(Type type, string value)
        {
            if (type == typeof(int))
            {
                bool err = !int.TryParse(value, out int intValue);

                return (err) ? intValue : null;
            }
            else if (type == typeof(long))
            {
                bool err = !long.TryParse(value, out long longValue);

                return (err) ? longValue : null;
            }
            else if (type == typeof(double))
            {
                bool err = !double.TryParse(value, out double doubleValue);

                return (err) ? doubleValue : null;
            }
            else if (type == typeof(bool))
            {
                return value.Contains("1");
            }
            else if (type == typeof(string))
            {
                return value;
            }

            return null;
        }
    }
}
