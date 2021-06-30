using Microsoft.AspNetCore.Builder;
using StorageDatabaseNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public static class CustomersExtention
    {
        public static IApplicationBuilder UseCustomers(this IApplicationBuilder builder, StorageDatabase storage)
        {
            return builder.UseMiddleware<CustomersMiddleware>(storage);
        }
    }
}
