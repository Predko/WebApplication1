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

        protected override string DeleteEntity { get => "/customers/remove"; }

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
                .Append("remove", "Удалить клиента")
                .Build();
        }

        /// <summary>
        /// Отображает список клиентов.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>false if an error is occured, otherwise - true</returns>
        protected override async Task<bool> ShowListOfEntities(HttpContext context, int unusedP1, int unusedP2)
        {
            StringBuilder response = new StringBuilder(Startup.BeginHtmlPages)
                    .Append("<main>")
                    .Append($"<div id='titleTable'><h1 id='h1ListEntities'> Список клиентов </h1></div>");

            string containerHeight = context.Request.Cookies["containerHeight"];

            if (containerHeight != null)
            {
                response.Append($"<div style='overflow-y:auto' id='divTable' style='height:{containerHeight}px'>");
            }
            else
            {
                response.Append($"<div style='overflow-y:auto' id='divTable'>");
            }

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

            response.Append($"<table id='list-entities' data-parameter='{EntityName}'><thead><tr>")
                    .Append($"<th width={maxlength[0]}ex data-sort-order='ascending'>УНП</th>")
                    .Append($"<th data-sort-order='ascending'>Название организации</th>")   //  width={columnsWidth[1]}%
                    .Append("</tr></thead>")
                    .Append(tbody).Append("</tbody>")
                    .Append("</table></div></main>")
                    .Append(ContextMenu)
                    .Append(Startup.EndHtmlPages)
                    .Append("<script src='/js/Scripts.js'></script><html>");   // <script src=""js\scripts.js"" type=""text/javascript""></script>

            await context.Response.WriteAsync(response.ToString());

            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns>False if an error is occured, otherwise - true</returns>
        protected override async Task<bool> ShowEditEntity(HttpContext context, int customerId, int unusedP)
        {
            DataTable dataTable = Storage[TableName, $"SELECT * FROM {TableName} WHERE Id='{customerId}'"];
            DataRow row = dataTable.Rows[0];

            StringBuilder response = GetEditEntityString(dataTable, row, "edit-customer", "Подробные данные клиента");

            await context.Response.WriteAsync(response.ToString());

            return true;
        }

        protected override async Task<bool> ShowNewEntity(HttpContext context, int customerId, int unusedP)
        {
            DataTable dataTable = Storage[TableName, $"SELECT * FROM {TableName} LIMIT 1"];
            DataRow newRow = dataTable.NewRow();

            newRow["Id"] = -1;

            StringBuilder response = GetEditEntityString(dataTable, newRow, "new-customer", "Новый клиент");

            await context.Response.WriteAsync(response.ToString());

            return true;
        }

        /// <summary>
        /// Возвращает разметку редактирования указанной в строке
        /// таблицы базы данных записи.
        /// </summary>
        /// <param name="dataTable">Таблица данных.</param>
        /// <param name="row">Строка для редактирования.</param>
        /// <returns></returns>
        private StringBuilder GetEditEntityString(DataTable dataTable, DataRow row, string className, string header)
        {
            StringBuilder response = new(string.Format(Startup.BeginHtmlPages,
                                                       "<link rel = \"stylesheet\" href= \"/Styles/Customers.css\" />"));

            if (row == null)
            {
                response.Append(@"<main><h1 color=""red"">Указанный клиент не найден</h1></main>")
                        .Append(Startup.EndHtmlPages + "</html>");
            }
            else
            {
                response.Append($"<main><h1>Подробные данные клиента</h1>")
                        .Append($"<form method='POST' id='formId'>")
                        .Append($"<input type='hidden' name='{CustomerEntityName}Id' value='{row["Id"]}' class='{className}'/>")

                        .Append("<table><tbody>");

                var columns = dataTable.Columns;

                for (int i = 1; i != row.ItemArray.Length; i++)
                {
                    response.Append($"<tr><td><label>{columnNames[i]}</label></td>")
                            .Append($"<td><input name='{columns[i].ColumnName}' value='{row[i]}'/></td></tr>");
                }

                response.Append("</tbody></table>")

                        .Append("<p><input type='submit' value='Отправить'>")
                        .Append("<input type='reset' value='Очистить'></p>")

                        .Append("</form>")
                        .Append("</main>")
                        .Append(Startup.EndHtmlPages).Append("<script src='/js/SubmitForm.js'></script><html>");
            }

            return response;
        }

        protected override async Task<bool> ShowDeleteEntity(HttpContext context, int customerId, int unusedP)
        {
            DataTable dataTable;

            // Обновить старую.
            dataTable = Storage["Customers", $"SELECT * FROM {TableName} WHERE Id='{customerId}'"];

            Storage.DeleteRecords(dataTable);

            dataTable.AcceptChanges();

            context.Response.Redirect(ListEntities);

            return await ShowListOfEntities(context, -1, -1);
        }

        protected override async Task<bool> ProcessFormRequest(HttpContext context, int customerId, int unusedP)
        {
            DataTable dataTable;
            DataRow row;

            if (customerId == -1)
            {
                // Добавить новую запись.
                dataTable = Storage["Customers", $"SELECT * FROM Customers LIMIT 1"];

                row = dataTable.NewRow();
            }
            else
            {
                // Обновить старую.
                dataTable = Storage["Customers", $"SELECT * FROM Customers WHERE Id='{customerId}'"];

                row = dataTable.Rows[0];
            }

            var formValues = context.Request.Form.Select(e => e.Value).ToArray();

            row.BeginEdit();

            for (int i = 1; i != row.ItemArray.Count(); i++)
            {
                Type type = dataTable.Columns[i].DataType;

                row[i] = GetRowValueFromString(type, formValues[i]);
            }

            row.EndEdit();

            if (customerId == -1)
            {
                // Добавляем новую запись.
                row["Id"] = -1;

                dataTable.Rows.Add(row);
            }

            Storage.UpdateDataTable("Customers");

            string message; 
            if (customerId == -1)
            {
                message = $"Клиент {row["NameCompany"]} добавлен успешно";
            }
            else
            {
                message = $"Данные клиента {row["NameCompany"]} обновлены успешно";
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/text; charset=UTF-8";

            await context.Response.WriteAsync(message);

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
