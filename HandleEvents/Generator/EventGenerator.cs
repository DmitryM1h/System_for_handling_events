using System.Reflection.Metadata.Ecma335;
using HandleEvents.Models;

namespace HandleEvents.Generator
{
    public class EventGenerator:IEventGenerator
    {
        private static readonly Random _random = new Random();

        public Event Generate()
        {
            var moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

            var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);

            var utcMoscowTime = TimeZoneInfo.ConvertTimeToUtc(moscowTime, moscowTimeZone);

            var ev = new Event(Guid.NewGuid(), (EventTypeEnum)_random.Next(1,4), utcMoscowTime);
            Console.WriteLine(ev);
            return ev;

        }

    }
}
