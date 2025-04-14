using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using EventProcessor.ModelsDTO;

namespace EventProcessor.Models
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


        [ForeignKey(nameof(IncidentId))]
        public Guid IncidentId { get; set; }

        [Required]
        public virtual Incident Incident { get; set; } = null!;


        public Event(EventReceive ev)
        {
            Id = Guid.NewGuid();
            Type = ev.Type;
            Time = ev.Time;
        }
        public Event(EventTypeEnum type,DateTime time,Guid incidentId)
        {
            Id = Guid.NewGuid();
            Type = type;
            Time = time;
            IncidentId = incidentId;
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
