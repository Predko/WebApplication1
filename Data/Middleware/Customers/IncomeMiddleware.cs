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
    public class IncomeMiddleware : AbstractCustomersMiddlware
    {
        protected override string TableName { get => "Income"; }

        protected override string EntityName { get => "income"; }

        protected override string ListEntities { get => "/customers/income"; }

        protected override string EditEntity { get => "/customers/income/edit"; }

        protected override string DeleteEntity { get => "/customers/income/delete"; }

        protected override string NewEntity { get => "/customers/income/new"; }

        protected override string ContextMenu { get; }

        public IncomeMiddleware(RequestDelegate next, StorageDatabase storage) : base(next, storage)
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
            DataView dataViewTable;
            string NameCustomer = "";

            try
            {
                string query;

                if (customerId != -1)
                {
                    foreach (DataRow r in Storage[CustomersTableName].Rows)
                    {
                        if ((long)r["Id"] == customerId)
                        {
                            NameCustomer = $" для {(string)r["NameCompany"]}";
                        }
                    }

                    query = $"SELECT * FROM {TableName} WHERE CustomerId = {customerId}";

                    dataViewTable = Storage[TableName, query]?.DefaultView;
                }
                else
                {
                    query = $"SELECT * FROM {TableName}";

                    dataViewTable = Storage[TableName]?.DefaultView;
                }

                if (dataViewTable == null)
                {
                    await context.Response.WriteAsync("Не удалось получить данные из базы данных по строке запроса:\n" +
                        query);
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
                    .Append($"<div id='titleTable'><h1 id='h1ListEntities'>Список поступлений{NameCustomer}</h1></div>")
                    .Append("<div style='overflow-y:auto' id='divTable'>")
                    .Append($"<table id='list-entities' data-parameter='{EntityName}'><thead><tr>")
                    .Append($"<th width={maxLength[0]}em data-sort-order='ascending'>Дата</th>")
                    .Append($"<th width={maxLength[0]}em data-sort-order='ascending'>Номер</th>")
                    .Append("<th data-sort-order='ascending'>Сумма</th>")
                    .Append("</tr></thead>")
                    .Append(tbody).Append("</tbody>")
                    .Append("</table></div></main>")
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

        //protected override async Task ShowEditEntity(HttpContext context, int customerId, int? incomeId)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override async Task ShowDeleteEntity(HttpContext context, int customerId, int? incomeId)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override async Task ShowNewEntity(HttpContext context, int customerId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
