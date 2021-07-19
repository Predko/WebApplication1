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
    public class ContractsMiddleware : AbstractCustomersMiddlware
    {
        protected override string TableName { get => "Contracts"; }

        protected override string EntityName { get => "contract"; }

        protected override string ListEntities { get => "/customers/contracts"; }

        protected override string EditEntity { get => "/customers/contracts/edit"; }

        protected override string DeleteEntity { get => "/customers/contracts/delete"; }

        protected override string NewEntity { get => "/customers/contracts/new"; }

        protected override string ContextMenu { get; }

        public ContractsMiddleware(RequestDelegate next, StorageDatabase storage) : base(next, storage)
        {
            ContextMenu = ContextMenuString.Builder()
                .Append("edit", "Просмотр и редактирование")
                .Append("new", "Добавить новый договор")
                .Append("delete", "Удалить договор")
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

                    return true;
                }

            }
            catch (SqliteException ex)
            {
                await context.Response.WriteAsync(ex.Message);

                return true;
            }

            dataViewTable.Sort = dataViewTable.Table.Columns[2].ColumnName; // Date

            StringBuilder tbody = new("<tbody>");

            const int columnsNumber = 4;
            int[] maxLength = new int[columnsNumber];
            string[] columns = new string[columnsNumber];

            foreach (DataRowView row in dataViewTable)
            {
                columns[0] = row["Date"] as string;
                columns[1] = row["Number"].ToString();
                columns[2] = row["Price"] as string;
                columns[3] = row["Available"].ToString() == "1" ? "Да" : "Нет";

                for (int i = 0; i != columnsNumber; i++)
                {
                    maxLength[i] = maxLength[i] >= columns[i].Length ? maxLength[i] : columns[i].Length;
                }

                tbody.Append(string.Format("<tr class='task' id='{0}'><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>",
                                                row["Id"].ToString(),
                                                columns[0],
                                                columns[1],
                                                columns[2],
                                                columns[3]));
            }

            StringBuilder response = new StringBuilder()
                    .Append(Startup.BeginHtmlPages)
                    .Append("<main>")
                    .Append($"<div id='titleTable'><h1 id='h1ListEntities'>Список договоров{NameCustomer}</h1></div>");

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
                    .Append($"<th width={maxLength[0]}ex data-sort-order='ascending'>Сумма</th>")
                    .Append("<th data-sort-order='ascending'>Наличие</th>")
                    .Append("</tr></thead>")
                    
                    .Append(tbody).Append("</tbody>")
                    
                    .Append("</table></div></main>")
                    .Append(ContextMenu)
                    .Append(Startup.EndHtmlPages)
                    .Append("<script src='/js/Scripts.js'></script><html>");   // <script src=""js\scripts.js"" type=""text/javascript""></script>

            await context.Response.WriteAsync(response.ToString());

            return true;
        }

        protected override async Task<bool> ShowDeleteEntity(HttpContext context, int customerId, int contractId)
        {
            DataTable dataTable;

            // Обновить старую.
            dataTable = Storage[TableName, $"SELECT * FROM {TableName} WHERE Id='{contractId}'"];

            Storage.DeleteRecords(dataTable);

            dataTable.AcceptChanges();

            context.Response.Redirect(ListEntities + $"?customer={customerId}");

            return true;
        }

        //protected override async Task<bool> ShowNewEntity(HttpContext context, int customerId, int p2)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
