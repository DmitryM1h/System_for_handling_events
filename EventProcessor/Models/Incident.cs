using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using EventProcessor.ModelsDTO;

namespace EventProcessor.Models
{
    public enum IncidentTypeEnum
    {
        Type1 = 1,
        Type2 = 2,
        Type3 = 3
    }
    public class Incident
    {

        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public IncidentTypeEnum Type { get; set; }

        [Required]
        public DateTime Time { get; set; }


        [Required]
        public virtual List<Event> Events { get; set; } = [];

        public Incident(Guid id,IncidentTypeEnum type, DateTime time)
        {
            Id = id;
            Type = type;
            Time = time;
        }
        public Incident(EventReceive ev)
        {
            Id = ev.Id;
            Type = (IncidentTypeEnum)ev.Type;
            Time = ev.Time;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Id: {Id}");
            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Time: {Time}");
            return sb.ToString();
        }

    }
}
