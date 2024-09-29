using System;
using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DataAccess;
using DomainModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BusMessageHandler
{
    public class ItemQueueHandler
    {
        private readonly ILogger<ItemQueueHandler> _logger;
        private ShoppingContext _dbContext;

        private void AddShoppingItem(ShoppingItem item)
        {
            _logger.LogInformation("Add item to database");

            using (var db = _dbContext)
            {
                db.ShoppingItems.Add(item);
                db.SaveChanges();

                // Display all Blogs from the database
                var query = from b in db.ShoppingItems
                            orderby b.Name
                            select b;
            }
        }

        private ShoppingItem TryDecodeItem(BinaryData data)
        {
            try
            {
                var item = JsonConvert.DeserializeObject<ShoppingItem>(data.ToString());
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to decode message body {ex}", ex.Message);

            }
            return new ShoppingItem { Name = data.ToString() };
        }


        public ItemQueueHandler(ILogger<ItemQueueHandler> logger, ShoppingContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            var serviceBusClientConnectionString = Environment.GetEnvironmentVariable("savvy-saves-bus");
            if (serviceBusClientConnectionString == null)
            {
                _logger.LogError("Unable to load bus connection string");
            }
        }

        [Function(nameof(ItemQueueHandler))]
        public async Task Run(
            [ServiceBusTrigger("items", Connection = "savvy-saves-bus")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
            _logger.LogInformation("Message Subject: {contentType}", message.Subject);
            _logger.LogInformation("Message EnqueuedTime: {EnqueuedTime}", message.EnqueuedTime);
            _logger.LogInformation("Message ExpiresAt: {ExpiresAt}", message.ExpiresAt);
            _logger.LogInformation("Message LockToken: {LockToken}", message.LockToken);
            _logger.LogInformation("Message EnqueuedSequenceNumber: {EnqueuedSequenceNumber}", message.EnqueuedSequenceNumber);
            _logger.LogInformation("Message SequenceNumber: {SequenceNumber}", message.SequenceNumber);
            _logger.LogInformation("Message LockedUntil: {LockedUntil}", message.LockedUntil);
            _logger.LogInformation("Message DeliveryCount: {DeliveryCount}", message.DeliveryCount);
            _logger.LogInformation("Message ScheduledEnqueueTime: {ScheduledEnqueueTime}", message.ScheduledEnqueueTime);
            _logger.LogInformation("Message ReplyTo: {ReplyTo}", message.ReplyTo);
            _logger.LogInformation("Message To: {To}", message.To);
            _logger.LogInformation("Message CorrelationId: {CorrelationId}", message.CorrelationId);
            _logger.LogInformation("Message TimeToLive: {TimeToLive}", message.TimeToLive);
            _logger.LogInformation("Message ReplyToSessionId: {ReplyToSessionId}", message.ReplyToSessionId);
            _logger.LogInformation("Message SessionId: {SessionId}", message.SessionId);
            _logger.LogInformation("Message TransactionPartitionKey: {TransactionPartitionKey}", message.TransactionPartitionKey);
            _logger.LogInformation("Message PartitionKey: {PartitionKey}", message.PartitionKey);

            var item = TryDecodeItem(message.Body);

            AddShoppingItem(item);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
