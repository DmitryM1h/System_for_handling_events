using System.ComponentModel.DataAnnotations;
using EventProcessor.db_context;
using EventProcessor.Models;
using EventProcessor.ModelsDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessorController : ControllerBase
    {

        private readonly IncidentContext _context;
        public ProcessorController(IncidentContext context)
        {
            _context = context;
        }

        // Возвращает все инциденты без событий
        [HttpGet("Incidents")]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidents()
        {
            return await _context.Incidents.ToListAsync();
        }

        
        // Возвращает все инциденты и их события
        [HttpGet("Incidents_With_Events")]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidentsEvents()
        {
            var incidents = await _context.Incidents.Include(t=>t.Events).ToListAsync();
            var incidentDto = incidents.Select(incident => new IncidentResponse
            {
                Incident_Id= incident.Id,
                Type = incident.Type,
                Time = incident.Time,
                Events = incident.Events.Select(e => new EventDto{ 
                    IncidentId = e.IncidentId,
                    EventId = e.Id,
                    EventType = e.Type,
                    EventDate = e.Time
                }).ToList()
            }).ToList();

            return Ok(incidentDto);
        }


        // Возвращает последние инциденты и их события в указанном количестве
        [HttpGet("Incidents_With_Events_Limited")]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidentsEvents(int limit)
        {
            if(limit < 0)
                return BadRequest("Invalid incident type.");

            var incidents = await _context.Incidents.Include(t => t.Events).OrderByDescending(t=>t.Time).Take(limit).ToListAsync();

            var incidentDto = incidents.Select(incident => new IncidentResponse
            {
                Incident_Id = incident.Id,
                Type = incident.Type,
                Time = incident.Time,
                Events = incident.Events.Select(e => new EventDto
                {
                    IncidentId = e.IncidentId,
                    EventId = e.Id,
                    EventType = e.Type,
                    EventDate = e.Time
                }).ToList()
            }).ToList();


            return Ok(incidentDto);
        }

        // Возвращает последние инциденты указанного типа в указанном количестве
        [HttpGet("Incidents_Of_Specified_Type")]
        public async Task<ActionResult<IEnumerable<Incident>>> GetSpecifiedIncidents(int type,int limit)
        {
           if(!Enum.IsDefined(typeof(IncidentTypeEnum), type)  || limit < 0)
            {
                return BadRequest("Invalid incident type.");
            }

            var incidents = await _context.Incidents.Include(t => t.Events).Where(t=>(int)t.Type==type).OrderByDescending(t => t.Time).Take(limit).ToListAsync();

            var incidentDto = incidents.Select(incident => new IncidentResponse
            {
                Incident_Id = incident.Id,
                Type = incident.Type,
                Time = incident.Time,
                Events = incident.Events.Select(e => new EventDto
                {
                    IncidentId = e.IncidentId,
                    EventId = e.Id,
                    EventType = e.Type,
                    EventDate = e.Time
                }).ToList()
            }).ToList();


            return Ok(incidentDto);
        }



        // Добавить инцидент в базу данных вручную

        [HttpPost("Add_Incident_Manually_To_Db")] 
        public async Task<ActionResult<Incident>> AddIncident(EventReceive eventDto)
        {
            if (eventDto == null)
            {
                return BadRequest("Invalid event data");
            }

            var existingIncident = await _context.Incidents
                .FirstOrDefaultAsync(t => t.Id == eventDto.Id);

            if (existingIncident != null)
            {
                return BadRequest("Event has already been received");
            }

            var incident = new Incident(eventDto.Id, (IncidentTypeEnum)eventDto.Type, eventDto.Time);
           
            await _context.Incidents.AddAsync(incident);

            var _event = new Event(eventDto);
            _event.IncidentId = incident.Id;

            await _context.Events.AddAsync(_event);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AddIncident), new { id = incident.Id, success = "True" });
        }


        
        // Отправить обработчику событий event указанного типа

        [HttpPost("Test_Manually")]
        public ActionResult TestManually(int type, [FromServices] EventProcessingService service)
        {
            if (!Enum.IsDefined(typeof(EventTypeEnum), type))
            {
                return BadRequest("Invalid event type.");
            }

            var ev = new EventReceive 
            { 
                Id = Guid.NewGuid(), 
                Time = DateTime.UtcNow,
                Type = (EventTypeEnum)type
            };
            service.AddEvent(ev);
            return Accepted(type);
        }


        // Принимает запросы от генератора событий
        // можно использовать этот эндпоинт в swagger, но с указанием даты и id

        [HttpPost("Events_From_Generator")]
        public async Task<IActionResult> ProcessEvent([FromBody] EventReceive received_event, [FromServices] EventProcessingService service)   
        {

            if (received_event == null)
            {
                return BadRequest("Invalid event data");
            }

            var existingIncident = await _context.Incidents.FirstOrDefaultAsync(t => t.Id == received_event.Id);

            if (existingIncident != null)
            {
                return BadRequest("Event has already been received");
            }

            service.AddEvent(received_event);
            return Accepted(received_event);
        }



    } 
}
