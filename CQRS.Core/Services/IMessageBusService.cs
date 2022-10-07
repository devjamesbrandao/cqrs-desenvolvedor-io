namespace CQRS.Core.Services
{
    public interface IMessageBusService
    {
        Task PublishMessageAsync(object entity);
    }
}