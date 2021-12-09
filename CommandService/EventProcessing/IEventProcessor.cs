namespace CommandService.EventProcesing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}