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
        protected override async Task<bool> ShowListOfEntities(HttpContext context, int customerId, int p2)
        {
            StringBuilder response = new(string.Format(Startup.BeginHtmlPages,
                                            "<link rel = 'stylesheet' href= '/Styles/Customers.css' />"));
            response.Append("<main>")
                    .Append("<div id='titleTable'><h1 id='h1ListEntities'>Список платежей</h1></div>")
                    .Append("<div style='overflow-y:auto' id='divTable'>")
                    .Append($"<table id='list-entities' data-parameter='{EntityName}'><thead><tr>")
                    .Append("<th data-sort-order='ascending'>Дата</th>")
                    .Append("<th data-sort-order='ascending'>Номер</th>")
                    .Append("<th data-sort-order='ascending'>Сумма</th>")
                    .Append("</tr></thead><tbody>");

            DataView dataViewTable;

            try
            {
                dataViewTable = Storage[TableName]?.DefaultView;

                if (dataViewTable == null)
                {
                    response.Clear();

                    await context.Response.WriteAsync("Не удалось получить данные из базы данных");
                }
            }
            catch (SqliteException ex)
            {
                response.Clear().Append(ex.Message);

                await context.Response.WriteAsync(response.ToString());

                return true;
            }

            dataViewTable.Sort = dataViewTable.Table.Columns[2].ColumnName; // Date

            foreach (DataRowView row in dataViewTable)
            {
                response.Append(string.Format("<tr class='task' id='{0}'><td>{1}</td><td>{2}</td><td>{3}</td></tr>",
                                                row["Id"].ToString(),
                                                row["Date"],
                                                row["Number"],
                                                row["Value"]));
            }

            response.Append("</tbody></table></div></main>")
                    .Append(ContextMenu)
                    .Append(Startup.EndHtmlPages)
                    .Append("<script src='/js/HandlerTS.js'></script><html>");   // <script src=""js\scripts.js"" type=""text/javascript""></script>

            await context.Response.WriteAsync(response.ToString());

            return true;
        }
        //protected override async Task<bool> ShowListOfEntities(HttpContext context, int customerId)
        //{
        //    throw new NotImplementedException(); return true;
        //}

        //protected override async Task ShowEditEntity(HttpContext context, int customerId, int? expensesId)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override async Task ShowDeleteEntity(HttpContext context, int customerId, int? expensesId)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override async Task ShowNewEntity(HttpContext context, int customerId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
