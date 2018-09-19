using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Websock.Database
{
    public static class DBinitialize
    {
        public static void EnsureCreated(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<MessageContext>().Database.EnsureCreated();
            }
        }
    }
}