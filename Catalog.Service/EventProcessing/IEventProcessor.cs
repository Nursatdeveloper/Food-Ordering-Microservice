namespace Catalog.Service.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string message);
    }
}