using System.Text;
using CQRS.Core.Entities;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CQRS.Infrastructure.MessageBus
{
    public class MessageBusConsumer : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly QueueClient _queueClient;
        private const string QUEUE_NAME = "product-queue";

        public MessageBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;

            var connectionString = _configuration.GetSection("ServiceBus:ConnectionString").Value;

            _queueClient = new QueueClient(connectionString, QUEUE_NAME);
        }

        public void RegisterHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionHandler)
            {
                AutoComplete = false
            };

            _queueClient.RegisterMessageHandler(ProcessMessageHandler, messageHandlerOptions);
        }

        private async Task ProcessMessageHandler(Message message, CancellationToken cancellationToken)
        {
            var messageString = Encoding.UTF8.GetString(message.Body);
            
            var userFollowingInputModel = JsonConvert.DeserializeObject<Product>(messageString);

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _queueClient?.CloseAsync();
        }
    }
}