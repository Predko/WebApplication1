using Microsoft.AspNetCore.Http;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class AbstractCustomersMiddlware
    {
        protected virtual string TableName { get; }

        protected virtual string ListEntities { get; }

        protected virtual string EditEntity { get; }

        protected virtual string DeleteEntity { get; }

        protected virtual string NewEntity { get; }

        protected virtual string EntityName { get => null; }

        protected virtual string CustomerEntityName { get => "customer"; }

        protected virtual string ContextMenu { get => "" +
@"<nav class='context-menu' id='context-menu'>
  <ul class='context-menu__items'>
    <li class='context-menu__item'>
      <a href = '#' class='context-menu__link' data-action='contracts'>
        <i class='fa fa-eye'></i>Договоры
      </a>
    </li>
    <li class='context-menu__item'>
      <a href = '#' class='context-menu__link' data-action='income'>
        <i class='fa fa-edit'></i>Оплата договоров
      </a>
    </li>
    <li class='context-menu__item'>
      <a href = '#' class='context-menu__link' data-action='edit'>
        <i class='fa fa-times'></i>Просмотр и редактирование
      </a>
    </li>
    <li class='context-menu__item'>
      <a href = '#' class='context-menu__link' data-action='delete'>
        <i class='fa fa-times'></i>Удалить
      </a>
    </li>
  </ul>
</nav>"; }


        protected RequestDelegate Next { get; }

        protected StorageDatabase Storage { get; }

        protected virtual string RequestFailed { get => "Ошибка в строке запроса"; }

        public AbstractCustomersMiddlware(RequestDelegate next, StorageDatabase storage)
        {
            Next = next;

            Storage = storage;
        }

        public virtual async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value.ToLower();

            context.Response.ContentType = "text/html;charset=utf-8";

            string customer_Id = context.Request.Query[CustomerEntityName];

            string entity_Id = "";

            if (EntityName != null)
            {
                entity_Id = context.Request.Query[EntityName];
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

            if (error)
            {
                await context.Response.WriteAsync(RequestFailed);
                return;
            }

            await Next.Invoke(context);
        }

        protected virtual async Task<bool> ShowListOfEntities(HttpContext context, int customerId)
        {
            await context.Response.WriteAsync("Список сущностей");

            return true;
        }

        protected virtual async Task ShowEditEntity(HttpContext context, int customerId, int? entityId)
        {
            await context.Response.WriteAsync("Редактирование сущности с id = " + entityId + ", клиента с id = " + customerId);
        }

        protected virtual async Task ShowDeleteEntity(HttpContext context, int customerId, int? entityId)
        {
            await context.Response.WriteAsync("Удаление сущности с id = " + entityId + ", клиента с id = " + customerId);
        }

        protected virtual async Task ShowNewEntity(HttpContext context, int customerId)
        {
            await context.Response.WriteAsync("Новая сущность для клиента с id = " + customerId);
        }

    }
}
