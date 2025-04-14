using HandleEvents.Models;
namespace HandleEvents.Generator
{
    public interface IEventGenerator
    {
        public Event Generate();
    }
}
