using Microsoft.AspNetCore.Http;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Buffers;

namespace WebApplication1.Data.Middleware.Customers
{
    public abstract class AbstractCustomersMiddlware
    {
        protected string CustomersTableName => "Customers";

        protected abstract string TableName { get; }

        protected abstract string ListEntities { get; }

        protected abstract string EditEntity { get; }

        protected abstract string DeleteEntity { get; }

        protected abstract string NewEntity { get; }

        protected abstract string EntityName { get; }

        protected virtual string CustomerEntityName { get => "customer"; }

        // For "POST" method, entity Id name = CustomerEntityName + "Id" and EntityName + "Id" respectively.
        // for "GET" method - CustomerEntityName and EntityName.

        private const string contextMenuBegin =
@"<nav class='context-menu' id='context-menu'>
  <ul class='context-menu__items'>";

        private const string contextMenuItem =
@"<li class='context-menu__item'>
      <p class='context-menu__link' data-action='{0}'>{1}</p>
    </li>";

        private const string contextMenuEnd =
@"  </ul>
</nav>";

        protected abstract string ContextMenu { get; }

        protected RequestDelegate Next { get; }

        protected StorageDatabase Storage { get; }

        protected virtual string RequestFailed { get => "Ошибка в строке запроса"; }

        public AbstractCustomersMiddlware(RequestDelegate next, StorageDatabase storage)
        {
            Next = next;

            Storage = storage;
        }

        /// <summary>
        /// Класс, реализующий создание контекстного меню.
        /// </summary>
        protected class ContextMenuString
        {
            private readonly StringBuilder menu;

            private ContextMenuString()
            {
                menu = new(contextMenuBegin);
            }

            public static ContextMenuString Builder() => new();

            public ContextMenuString Append(string dataAction, string itemContent)
            {
                menu.Append(string.Format(contextMenuItem, dataAction, itemContent));

                return this;
            }

            public string Build() => menu.Append(contextMenuEnd).ToString();
        }



        private delegate Task<bool> ShowDelegate(HttpContext context, int entityParent, int entityChild);

        public class Param
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public Param(string jsonString)
            {
                var sa = jsonString.Trim('{', '}').Split(':');

                Name = sa[0]?.Trim();
                Value = sa[1]?.Trim();
            }

        }

        protected List<Param> listParams;

        /// <summary>
        /// Обработчик компонента Middleware.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task InvokeAsync(HttpContext context)
        {
            ShowDelegate ShowPage = null;

            string path = context.Request.Path.Value.ToLower();

            if (path == ListEntities)
            {
                ShowPage = ShowListOfEntities;
            }
            else if (path == EditEntity)
            {
                ShowPage = ShowEditEntity;
            }
            else if (path == DeleteEntity)
            {
                ShowPage = ShowDeleteEntity;
            }
            else if (path == NewEntity)
            {
                ShowPage = ShowNewEntity;
            }
            else if (path.EndsWith("submit") == true)
            {
                ShowPage = ProcessFormRequest;
            }

            // For "POST" method, entity Id name = CustomerEntityName + "Id" and EntityName + "Id" respectively.
            // for "GET" method - CustomerEntityName and EntityName.
            string GetParameter(string nameParam) => (context.Request.Method == "GET") ? context.Request.Query[nameParam]
                                                                                       : context.Request.Form[nameParam + "Id"];
            //-------

            context.Response.ContentType = "text/html;charset=utf-8";

            string customer_Id = GetParameter(CustomerEntityName);

            if (string.IsNullOrWhiteSpace(customer_Id) == true || int.TryParse(customer_Id, out int customerId) == false)
            {
                customerId = -1;
            }

            string entity_Id = "";

            if (EntityName != null)
            {
                entity_Id = GetParameter(EntityName);
            }

            if (string.IsNullOrWhiteSpace(entity_Id) == true || int.TryParse(entity_Id, out int entityId) == false)
            {
                entityId = -1;
            }

            bool error = false;

            if (ShowPage != null)
            {
                error = await ShowPage(context, customerId, entityId);

                if (!error)
                {
                    // была ошибка и она необработана.

                    await context.Response.WriteAsync(RequestFailed);
                }

                return;
            }

            await Next.Invoke(context);
        }

        /// <summary>
        /// Отображает на странице список сущностей.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <returns>If an error occurred and it was not handled, otherwise - true</returns>
        protected virtual async Task<bool> ShowListOfEntities(HttpContext context, int customerId, int p2)
        {
            await context.Response.WriteAsync("Список сущностей");

            return true;
        }

        /// <summary>
        /// Редактирование сущности.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <param name="entityId"></param>
        /// <returns>If an error occurred and it was not handled, otherwise - true</returns>
        protected virtual async Task<bool> ShowEditEntity(HttpContext context, int customerId, int entityId)
        {
            await context.Response.WriteAsync("Редактирование сущности с id = " + entityId + ", клиента с id = " + customerId);

            return true;
        }

        /// <summary>
        /// Удаление сущности.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <param name="entityId"></param>
        /// <returns>If an error occurred and it was not handled, otherwise - true</returns>
        protected virtual async Task<bool> ShowDeleteEntity(HttpContext context, int customerId, int entityId)
        {
            await context.Response.WriteAsync("Удаление сущности с id = " + entityId + ", клиента с id = " + customerId);

            return true;
        }

        /// <summary>
        /// Добавляет новую сущность.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <returns>If an error occurred and it was not handled, otherwise - true</returns>
        protected virtual async Task<bool> ShowNewEntity(HttpContext context, int customerId, int p2)
        {
            await context.Response.WriteAsync("Новая сущность для клиента с id = " + customerId);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <param name="p2"></param>
        /// <returns>If an error occurred and it was not handled, otherwise - true</returns>
        protected virtual async Task<bool> ProcessFormRequest(HttpContext context, int customerId, int p2)
        {
            await context.Response.WriteAsync("Новая сущность для клиента с id = " + customerId + " создана.");

            return true;
        }
    }
}
