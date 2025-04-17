using EventProcessor.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventProcessor.ModelsDTO
{
    public class EventReceive
    {

        [Required]
        public Guid Id { get; set; }

        [Required]
        [Range(1, 3, ErrorMessage = "Type must be between 1 and 3")]
        public EventTypeEnum Type { get; set; }
        [Required]
        public DateTime Time { get; set; }

        public EventReceive() { }

        public EventReceive(Guid id, EventTypeEnum type, DateTime time)
        {
            Id = id;
            Type = type;
            Time = time;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Event ID: {Id}");
            sb.AppendLine($"Event Type: {Type}");
            sb.AppendLine($"Event Time: {Time}");
            return sb.ToString();

        }
    }
}
