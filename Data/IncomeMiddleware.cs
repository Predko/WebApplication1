using Microsoft.AspNetCore.Http;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public class IncomeMiddleware
    {
        private RequestDelegate Next { get; }

        private StorageDatabase Storage { get; }

        public IncomeMiddleware(RequestDelegate next, StorageDatabase storage)
        {
            Next = next;
            Storage = storage;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path;

            context.Response.ContentType = "text/html; charset=utf-8";


            string customer_Id = context.Request.Query["customer"];
            string income_Id = context.Request.Query["income"];

            if (string.IsNullOrWhiteSpace(customer_Id) == true || int.TryParse(customer_Id, out int customerId) == false)
            {
                customerId = 0;
            }

            if (string.IsNullOrWhiteSpace(income_Id) == true || int.TryParse(income_Id, out int incomeId) == false)
            {
                incomeId = 0;
            }

            switch (path)
            {
                case "/customers/income":
                    await ShowListIncome(context);
                    break;

                case "/customers/income/edit":
                    if (customerId != 0 && incomeId != 0)
                    {
                        await ShowEditIncome(context, customerId, incomeId);
                    }
                    break;

                case "/customers/income/delete":
                    if (customerId != 0 && incomeId != 0)
                    {
                        await ShowDeleteIncome(context, customerId, incomeId);
                    }

                    break;

                case "/customers/income/new":
                    if (customerId != 0)
                    {
                        await ShowNewIncome(context, customerId);
                    }

                    break;

                default:

                    await Next.Invoke(context);
                    break;
            }


        }

        private Task ShowNewIncome(HttpContext context, int customerId)
        {
            throw new NotImplementedException();
        }

        private Task ShowDeleteIncome(HttpContext context, int customerId, int incomeId)
        {
            throw new NotImplementedException();
        }

        private Task ShowEditIncome(HttpContext context, int customerId, int incomeId)
        {
            throw new NotImplementedException();
        }

        private Task ShowListIncome(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
