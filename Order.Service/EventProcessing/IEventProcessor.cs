namespace Order.Service.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string message);
    }
}
