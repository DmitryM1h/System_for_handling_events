using System.Reflection.Metadata.Ecma335;
using HandleEvents.Models;

namespace HandleEvents.Generator
{
    public class EventGenerator:IEventGenerator
    {
        private static readonly Random _random = new Random();

        public Event Generate()
        {

            var ev = new Event(Guid.NewGuid(), (EventTypeEnum)_random.Next(1,4), DateTime.Now.ToUniversalTime());
            Console.WriteLine(ev);
            return ev;

        }

    }
}
