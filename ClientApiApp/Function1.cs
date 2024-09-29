using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ClientApiApp
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private ShoppingContext _dbContext;


        public Function1(ILogger<Function1> logger, ShoppingContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");


            // Display all Blogs from the database
            var query = _dbContext.ShoppingItems.OrderBy(o => o.Name).Take(10);

            return new OkObjectResult(query);
        }
    }
}
