using DataAccess;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Retrieve the connection string for use with the application. 
string connectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_CONNECTIONSTRING");

if (connectionString == null)
{
    Console.WriteLine("Azure BLOB connection string not found.");
}


var sqlConnection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");

if (sqlConnection == null)
{
    Console.WriteLine("Azure SQL connection string not found.");
}

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<ShoppingContext>(options => options.UseSqlServer(sqlConnection, b => b.MigrationsAssembly("ClientApiApp")));
    })
    .Build();

host.Run();
