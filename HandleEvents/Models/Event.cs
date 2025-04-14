using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HandleEvents.Models
{
    public enum EventTypeEnum
    {
        Type1 = 1,
        Type2 = 2,
        Type3 = 3
    }

    public class Event
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [Range(1, 3, ErrorMessage = "Type must be between 1 and 3")]
        public EventTypeEnum Type { get; set; }
        [Required]
        public DateTime Time { get; set; }

        public Event(Guid id, EventTypeEnum type, DateTime time)
        {
            Id = id;
            Type = type;
            Time = time;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Event ID: {Id}");
            sb.AppendLine($"Event Type: {Type}");
            sb.AppendLine($"Event Time: {Time}");
            return sb.ToString();
        }
    }
}
