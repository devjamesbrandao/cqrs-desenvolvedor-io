using System.Text;
using CQRS.Core.Services;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CQRS.Infrastructure.MessageBus
{
    public class MessageBusService : IMessageBusService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly QueueClient _queueClient;
        private const string QUEUE_NAME = "product-queue";

        public MessageBusService(IConfiguration configuration, QueueClient queueClient)
        {
            _configuration = configuration;

            var connectionString = _configuration.GetSection("ServiceBus:ConnectionString").Value;

            _queueClient = new QueueClient(connectionString, QUEUE_NAME);
        }

        public async Task PublishMessageAsync(object entity)
        {
            var entityString = JsonConvert.SerializeObject(entity);

            var messageBytes = Encoding.UTF8.GetBytes(entityString);

            var message = new Message(messageBytes);

            await _queueClient.SendAsync(message);
        }

        public void Dispose()
        {
            _queueClient?.CloseAsync();
        }
    }
}