using Microsoft.AspNetCore.Http;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public class ContractsMiddleware: AbstractCustomersMiddlware
    {
        protected override string TableName { get => "Contracts"; }

        protected override string EntityName { get => "contract"; }

        protected override string ListEntities { get => "/customers/contracts"; }

        protected override string EditEntity { get => "/customers/contracts/edit"; }

        protected override string DeleteEntity { get => "/customers/contracts/delete"; }

        protected override string NewEntity { get => "/customers/contracts/new"; }

        protected override string ContextMenu { get; }

        public ContractsMiddleware(RequestDelegate next, StorageDatabase storage): base(next, storage)
        {
            ContextMenu = ContextMenuString.GetBuilder()
                .Append("edit", "Просмотр и редактирование")
                .Append("new", "Добавить новый договор")
                .Append("delete", "Удалить договор")
                .GetContextMenuString();
        }

        //protected override async Task<bool> ShowListOfEntities(HttpContext context, int id, int p2)
        //{
        //    await context.Response.WriteAsync("Список договоров"); return true;
        //}

        //protected override async Task<bool> ShowDeleteEntity(HttpContext context, int customerId, int contractId)
        //{
        //    await context.Response.WriteAsync("Редактирование договора с id = " + contractId + ", клиента с id = " + customerId);
        //}

        //protected override async Task<bool> ShowNewEntity(HttpContext context, int customerId, int p2)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
