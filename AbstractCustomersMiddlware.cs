using Microsoft.AspNetCore.Http;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1
{
    public abstract class AbstractCustomersMiddlware
    {
        protected virtual string TableName { get; }

        protected virtual string ListEntities { get; }

        protected virtual string EditEntity { get; }

        protected virtual string DeleteEntity { get; }

        protected virtual string NewEntity { get; }

        protected virtual string EntityName { get => null; }

        protected virtual string CustomerEntityName { get => "customer"; }

        private const string contextMenuBegin =
@"<nav class='context-menu' id='context-menu'>
  <ul class='context-menu__items'>";

        private const string contextMenuItem =
@"<li class='context-menu__item'>
      <a href = '#' class='context-menu__link' data-action='{0}'>{1}</a>
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

            public static ContextMenuString GetBuilder() => new();

            public ContextMenuString Append(string dataAction, string itemContent)
            {
                menu.Append(string.Format(contextMenuItem, dataAction, itemContent));

                return this;
            }

            public string GetContextMenuString() => menu.Append(contextMenuEnd).ToString();
        }

        /// <summary>
        /// Обработчик компонента Middleware.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value.ToLower();

            context.Response.ContentType = "text/html;charset=utf-8";

            string customer_Id = context.Request.Query[CustomerEntityName];

            string entity_Id = "";

            if (EntityName != null)
            {
                entity_Id = context.Request.Query[EntityName];
                if (context.Request.Method == "POST")
                {
                    customer_Id = context.Request.Form.FirstOrDefault(p => p.Key == "Id").Value;
                }
            }

            if (string.IsNullOrWhiteSpace(customer_Id) == true || int.TryParse(customer_Id, out int customerId) == false)
            {
                customerId = 0;
            }

            int? entityId = null;

            if (string.IsNullOrWhiteSpace(entity_Id) == false && int.TryParse(entity_Id, out int id) == true)
            {
                entityId = id;
            }

            bool error = false;

            if (path == ListEntities)
            {
                error = await ShowListOfEntities(context, customerId);

                if (!error)
                {
                    return;
                }
            }
            else if (path == EditEntity)
            {
                if (customerId != 0)
                {
                    await ShowEditEntity(context, customerId, entityId);

                    return;
                }

                error = true;
            }
            else if (path == DeleteEntity)
            {
                if (customerId != 0)
                {
                    await ShowDeleteEntity(context, customerId, entityId);

                    return;
                }

                error = true;
            }
            else if (path == NewEntity)
            {
                if (customerId != 0)
                {
                    await ShowNewEntity(context, customerId);

                    return;
                }

                error = true;
            }
            else if (path.EndsWith("submit") == true)
            {
                //if (customerId != 0)
                //{
                customer_Id = context.Request.Form.FirstOrDefault(p => p.Key == "Id").Value;


                
                await ProcessRequest(context, customerId);

                    return;
                //}

                error = true;
            }

            if (error)
            {
                await context.Response.WriteAsync(RequestFailed);
                return;
            }

            await Next.Invoke(context);
        }

        /// <summary>
        /// Отображает на странице список сущностей.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        protected virtual async Task<bool> ShowListOfEntities(HttpContext context, int customerId)
        {
            await context.Response.WriteAsync("Список сущностей");

            return false;
        }

        /// <summary>
        /// Редактирование сущности.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        protected virtual async Task ShowEditEntity(HttpContext context, int customerId, int? entityId)
        {
            await context.Response.WriteAsync("Редактирование сущности с id = " + entityId + ", клиента с id = " + customerId);
        }

        /// <summary>
        /// Удаление сущности.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        protected virtual async Task ShowDeleteEntity(HttpContext context, int customerId, int? entityId)
        {
            await context.Response.WriteAsync("Удаление сущности с id = " + entityId + ", клиента с id = " + customerId);
        }

        /// <summary>
        /// Добавляет новую сущность.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        protected virtual async Task ShowNewEntity(HttpContext context, int customerId)
        {
            await context.Response.WriteAsync("Новая сущность для клиента с id = " + customerId);
        }

        protected virtual async Task ProcessRequest(HttpContext context, int customerId)
        {
            await context.Response.WriteAsync("Новая сущность для клиента с id = " + customerId);
        }
    }
}
