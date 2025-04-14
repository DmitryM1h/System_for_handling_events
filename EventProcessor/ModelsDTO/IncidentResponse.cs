using EventProcessor.Models;
namespace EventProcessor.ModelsDTO
{
    public class EventDto
    {
        public Guid IncidentId { get; set; }
        public Guid EventId { get; set; }
        public EventTypeEnum EventType { get; set; } 
        public DateTime EventDate { get; set; }
    }

    public class IncidentResponse
    {
        public Guid Incident_Id { get; set; } 
        public List<EventDto> Events { get; set; } = new List<EventDto>();

        public IncidentTypeEnum Type { get; set; }

        public DateTime Time { get; set; }

 
    }
}
