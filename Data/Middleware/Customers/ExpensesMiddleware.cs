using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Data.Middleware.Customers
{
    public class ExpensesMiddleware : AbstractCustomersMiddlware
    {
        protected override string TableName { get => "Expenses"; }

        protected override string EntityName { get => "expenses"; }

        protected override string ListEntities { get => "/customers/expenses"; }

        protected override string EditEntity { get => "/customers/expenses/edit"; }

        protected override string DeleteEntity { get => "/customers/expenses/delete"; }

        protected override string NewEntity { get => "/customers/expenses/new"; }

        protected override string ContextMenu { get; }

        public ExpensesMiddleware(RequestDelegate next, StorageDatabase storage) : base(next, storage)
        {
            ContextMenu = ContextMenuString.Builder()
                .Append("edit", "Просмотр и редактирование")
                .Append("new", "Добавить новое поступление")
                .Append("delete", "Удалить запись")
                .Build();
        }

        /// <summary>
        /// Отображает список клиентов.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>false if an error is occured, otherwise - true</returns>
        protected override async Task<bool> ShowListOfEntities(HttpContext context, int customerId, int unusedP)
        {
            DataView dataViewTable;

            try
            {
                dataViewTable = Storage[TableName]?.DefaultView;

                if (dataViewTable == null)
                {
                    await context.Response.WriteAsync("Не удалось получить данные из базы данных");
                }
            }
            catch (SqliteException ex)
            {
                await context.Response.WriteAsync(ex.Message);

                return true;
            }

            dataViewTable.Sort = dataViewTable.Table.Columns[2].ColumnName; // Date

            StringBuilder tbody = new("<tbody>");

            const int columnsNumber = 3;
            int[] maxLength = new int[columnsNumber];
            string[] columns = new string[columnsNumber];

            foreach (DataRowView row in dataViewTable)
            {
                columns[0] = row["Date"] as string;
                columns[1] = row["Number"].ToString();
                columns[2] = row["Value"] as string;

                for (int i = 0; i != columnsNumber; i++)
                {
                    maxLength[i] = maxLength[i] >= columns[i].Length ? maxLength[i] : columns[i].Length;
                }

                tbody.Append(string.Format("<tr class='task' id='{0}'><td>{1}</td><td>{2}</td><td>{3}</td></tr>",
                                                row["Id"].ToString(),
                                                columns[0],
                                                columns[1],
                                                columns[2]));
            }

            StringBuilder response = new StringBuilder(Startup.BeginHtmlPages)
                    .Append("<main>")
                    .Append("<div id='titleTable'><h1 id='h1ListEntities'>Список платежей</h1></div>");

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
                    .Append($"<th width={maxLength[0]}ex data-sort-order='ascending'>Дата</th>")
                    .Append($"<th width={maxLength[0]}ex data-sort-order='ascending'>Номер</th>")
                    .Append($"<th data-sort-order='ascending'>Сумма</th>")
                    .Append("</tr></thead>")
                    .Append(tbody).Append("</tbody>")
                    .Append("</tbody></table></div></main>")
                    .Append(ContextMenu)
                    .Append(Startup.EndHtmlPages)
                    .Append("<script src='/js/Scripts.js'></script><html>");   // <script src=""js\scripts.js"" type=""text/javascript""></script>

            await context.Response.WriteAsync(response.ToString());

            return true;
        }

        protected override async Task<bool> ShowDeleteEntity(HttpContext context, int unusedP, int expensesId)
        {
            DataTable dataTable;

            // Обновить старую.
            dataTable = Storage[TableName, $"SELECT * FROM {TableName} WHERE Id='{expensesId}'"];

            Storage.DeleteRecords(dataTable);

            dataTable.AcceptChanges();

            context.Response.Redirect(ListEntities);

            return await ShowListOfEntities(context, -1, -1);
        }

        //protected override async Task ShowEditEntity(HttpContext context, int customerId, int? expensesId)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override async Task ShowNewEntity(HttpContext context, int customerId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
