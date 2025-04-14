using EventProcessor.Models;
using System;

// EventProcessor/Services/EventProcessingService.cs
using EventProcessor.ModelsDTO;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;
using EventProcessor.db_context;
using Microsoft.EntityFrameworkCore;

namespace EventProcessor
{
    public class EventProcessingService : BackgroundService
    {
        readonly List<EventReceive> _events = [];

        private readonly IServiceProvider _service;

        bool waiting_for_type1 = false;
        bool waiting_for_type2 = false;

        public EventProcessingService(IServiceProvider service)
        {
            _service = service;
        }


        public void AddEvent(EventReceive _event)
        {
            Console.WriteLine("Добавлен новый event" + _event);
            _events.Add(_event);
            Task.Run(() => ProcessEvent(_event));

        }


        public async Task ProcessEvent(EventReceive _event)
        {
            using (var scope = _service.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<IncidentContext>();

                switch (_event.Type)
                {
                    case EventTypeEnum.Type1:
                        await HandleType1Event(_event, _context);
                        break;

                    case EventTypeEnum.Type2:
                        await HandleType2Event(_event, _context);
                        break;

                    case EventTypeEnum.Type3:
                        await HandleType3Event(_event, _context);
                        break;
                }
            }
        }

        private async Task HandleType1Event(EventReceive _event, IncidentContext _context)
        {
            if (waiting_for_type1) return;

            Console.WriteLine("Возвращаю type1 сразу");
            var incident = new Incident(_event);
            var ev = new Event(_event) { IncidentId = incident.Id };

            await _context.Incidents.AddAsync(incident);
            await _context.Events.AddAsync(ev);
            await _context.SaveChangesAsync();
        }

        private async Task HandleType2Event(EventReceive _event, IncidentContext _context)
        {
            if (waiting_for_type2) return;

            waiting_for_type1 = true;
            var task = Wait_for_event(_event, EventTypeEnum.Type1);
            await task;

            if (task.Result != null)
            {
                await CreateIncidentWithEvents(_event, task.Result, _context);
            }
            else
            {
                await CreateDefaultIncident(_event, IncidentTypeEnum.Type1, _context);
            }
            waiting_for_type1 = false;
        }

        private async Task HandleType3Event(EventReceive _event, IncidentContext _context)
        {
            waiting_for_type2 = true;
            var task = Wait_for_event(_event, EventTypeEnum.Type2);
            await task;

            if (task.Result != null)
            {
                await CreateIncidentWithEvents(_event, task.Result, _context);
            }
            else
            {
                await CreateDefaultIncident(_event, IncidentTypeEnum.Type1, _context);
            }
            waiting_for_type2 = false;
        }
         
        private async Task CreateIncidentWithEvents(EventReceive _event, EventReceive resultEvent, IncidentContext _context)
        {
            var incident = new Incident(_event);
            var ev1 = new Event(_event) { IncidentId = incident.Id };
            var ev2 = new Event(resultEvent) { IncidentId = incident.Id };

            await _context.Incidents.AddAsync(incident);
            await _context.Events.AddAsync(ev1);
            await _context.Events.AddAsync(ev2);
            await _context.SaveChangesAsync();
        }

        private async Task CreateDefaultIncident(EventReceive _event, IncidentTypeEnum incidentType, IncidentContext _context)
        {
            var incident = new Incident(_event) { Type = incidentType };
            var ev = new Event(_event) { IncidentId = incident.Id };

            await _context.Incidents.AddAsync(incident);
            await _context.Events.AddAsync(ev);
            await _context.SaveChangesAsync();
        }

        public async Task<EventReceive?> Wait_for_event(EventReceive _event,EventTypeEnum type)
        {
            int index = _events.IndexOf(_event);
            int seconds = type == EventTypeEnum.Type1 ? 20 : 60;
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(seconds));
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine($"Ожидаем type {type}");
                var newEvents = _events.Skip(index + 1).Where(e => e.Type == type).ToList();
                var exists = newEvents.FirstOrDefault();
                if (exists != null)
                {
                    Console.WriteLine($"type {type} найден!");
                    return exists;
                }
                await Task.Delay(500);
            }
            Console.WriteLine($"Время ожидания истекло. Событие типа {type} не найдено.");
            return null;
        }
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                RemoveOldEvents();

                if (_events.Any())
                {
                    Console.WriteLine("Contains : ");
                    foreach (var ev in _events)
                        Console.WriteLine($"{ev}");

                }
                else
                {
                    Console.WriteLine("Ожидаю");
                }
                await Task.Delay(1000, stoppingToken);

            }
        }
        public void RemoveOldEvents()
        {
            DateTime r = DateTime.Now.ToUniversalTime().AddSeconds(-70);
            var old = _events.Where(t => t.Time < r).ToList();
            if (old != null && old.Count != 0)
            {
                Console.WriteLine("Очистка обработанных событий");
                _events.RemoveAll(e => e.Time < r);
            }
        }
    }
}