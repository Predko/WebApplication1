using Microsoft.AspNetCore.Builder;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data.Middleware.Customers;

namespace WebApplication1.Data.Middleware
{
    public static class CustomersExtention
    {
        public static IApplicationBuilder UseCustomers(this IApplicationBuilder builder, StorageDatabase storage)
        {
            return builder.UseMiddleware<CustomersMiddleware>(storage);
        }

        public static IApplicationBuilder UseContracts(this IApplicationBuilder builder, StorageDatabase storage)
        {
            return builder.UseMiddleware<ContractsMiddleware>(storage);
        }

        public static IApplicationBuilder UseIncome(this IApplicationBuilder builder, StorageDatabase storage)
        {
            return builder.UseMiddleware<IncomeMiddleware>(storage);
        }
    }
}
