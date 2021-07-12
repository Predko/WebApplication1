using Microsoft.AspNetCore.Http;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public class IncomeMiddleware: AbstractCustomersMiddlware
    {
        protected override string TableName { get => "Income"; }

        protected override string EntityName { get => "income"; }

        protected override string ListEntities { get => "/customers/income"; }

        protected override string EditEntity { get => "/customers/income/edit"; }

        protected override string DeleteEntity { get => "/customers/income/delete"; }

        protected override string NewEntity { get => "/customers/income/new"; }

        public IncomeMiddleware(RequestDelegate next, StorageDatabase storage): base(next, storage)
        {
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
