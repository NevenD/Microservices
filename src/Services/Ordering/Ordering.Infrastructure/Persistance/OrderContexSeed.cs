using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistance
{
    public  class OrderContexSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContexSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order() {UserName = "Neven", FirstName = "Neven", LastName = "Dimač", EmailAddress = "ezozkme@gmail.com", AddressLine = "Pregrada", Country = "Hrvatska", TotalPrice = 350 }
            };
        }
    }
}
