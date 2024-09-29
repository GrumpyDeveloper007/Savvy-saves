using DataAccess;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");

if (connection == null)
{
    Console.WriteLine("Azure SQL connection string not found.");
}

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<ShoppingContext>(options => options.UseSqlServer(connection, b => b.MigrationsAssembly("ClientApiApp")));
    })
    .Build();

host.Run();
