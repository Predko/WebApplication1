using Microsoft.AspNetCore.Http;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public class ContractsMiddleware
    {
        private RequestDelegate Next { get; }

        private StorageDatabase Storage { get; }

        public ContractsMiddleware(RequestDelegate next, StorageDatabase storage)
        {
            Next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value.ToLower();

            context.Response.ContentType = "text/html;charset=utf-8";

            string customer_Id = context.Request.Query["customer"];

            string contract_Id = context.Request.Query["contract"];

            if (string.IsNullOrWhiteSpace(customer_Id) == true || int.TryParse(customer_Id, out int customerId) == false)
            {
                customerId = 0;
            }

            if (string.IsNullOrWhiteSpace(contract_Id) == true || int.TryParse(contract_Id, out int contractId) == false)
            {
                contractId = 0;
            }

            switch (path)
            {
                case "/customers/contracts":

                    if (customerId != 0)
                    {
                        await ShowListContracts(context, customerId);
                        return;
                    }

                    await context.Response.WriteAsync("Ошибка в строке запроса");
                    return;

                case "/customers/contracts/edit":

                    if (customerId != 0 && contractId != 0)
                    {
                        await ShowDeleteContract(context, customerId, contractId);

                        return;
                    }

                    await context.Response.WriteAsync("Ошибка в строке запроса");
                    return;

                case "/customers/contracts/delete":

                    if (customerId != 0 && contractId != 0)
                    {
                        await ShowDeleteContract(context, customerId, contractId);

                        return;
                    }

                    await context.Response.WriteAsync("Ошибка в строке запроса");
                    return;

                case "/customers/contracts/new":

                    if (customerId != 0)
                    {
                        await ShowNewContract(context, customerId);

                        return;
                    }

                    await context.Response.WriteAsync("Ошибка в строке запроса");
                    return;

                default:
                    break;
            }

            await Next.Invoke(context);
        }

        private Task ShowNewContract(HttpContext context, int customerId)
        {
            throw new NotImplementedException();
        }

        private async Task ShowDeleteContract(HttpContext context, int customerId, int contractId)
        {
            await context.Response.WriteAsync("Редактирование договора с id = " + contractId + ", клиента с id = " + customerId);
        }

        private async Task ShowListContracts(HttpContext context, int id)
        {
            await context.Response.WriteAsync("Список договоров");
        }
    }
}
