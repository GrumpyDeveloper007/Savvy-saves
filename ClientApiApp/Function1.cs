using Azure.Storage.Blobs;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

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

        public class MultiPartFormDataModel
        {
            public byte[] FileUpload { get; set; }
        }

        // TODO: Separate class?
        [Function("Upload")]
        [OpenApiOperation(operationId: "Upload")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "multipart/form-data", bodyType: typeof(MultiPartFormDataModel), Required = true, Description = "Image data")]
        public async Task<IActionResult> Upload([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("upload triggered");

            try
            {
                var formdata = await req.ReadFormAsync();
                var file = req.Form.Files[0];

                await UploadToBlob(file);
                return new OkObjectResult("Goodday mate!, " + file.FileName + " - " + file.Length.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Upload failed, {ex.Message}");
                return new BadRequestObjectResult(ex);
            }
        }

        private async Task UploadToBlob(IFormFile file)
        {
            // Retrieve the connection string for use with the application. 
            string connectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_CONNECTIONSTRING");


            // Create a BlobServiceClient object 
            var blobServiceClient = new BlobServiceClient(connectionString);

            var containerClient = blobServiceClient.GetBlobContainerClient("images");


            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(file.FileName);
            var fileStream = file.OpenReadStream();
            _logger.LogInformation("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);


            // Upload data from the local file, overwrite the blob if it already exists
            await blobClient.UploadAsync(fileStream);

        }
    }
}
