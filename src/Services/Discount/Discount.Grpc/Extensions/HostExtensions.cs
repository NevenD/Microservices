using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Threading;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailiability = retry.Value;

            using (IServiceScope scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgresql...");

                    using NpgsqlConnection conn
                      = new(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    // we should open explicitly conn  when we want to perform database operations
                    conn.Open();

                    using NpgsqlCommand command = new()
                    {
                        Connection = conn
                    };


                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated postresql database.");
                }
                catch (NpgsqlException ex)
                {

                    logger.LogInformation(ex,"Error occured while migrating postgresql database");

                    if (retryForAvailiability < 50)
                    {
                        retryForAvailiability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailiability);
                    }
                }
            }

            return host;
        }
    }
}
