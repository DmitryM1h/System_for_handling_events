
using HandleEvents.Generator;
using HandleEvents.Controllers;
using HandleEvents.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace HandleEvents.EventGenerationService
{
    public class EventGenerationService : BackgroundService
    {
        private readonly IEventGenerator _eventGenerator;
        private readonly IHttpClientFactory _httpClientFactory;

        public EventGenerationService(IEventGenerator eventGenerator, IHttpClientFactory httpClientFactory)
        {
            _eventGenerator = eventGenerator;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var generatedEvent = _eventGenerator.Generate();
                await SendEventToProcessor(generatedEvent);
                Console.WriteLine("Сгенерирован event в ExecuteAsync" + generatedEvent);

                await Task.Delay(2000, stoppingToken);
            }
        }

        private async Task SendEventToProcessor(Event generatedEvent)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(generatedEvent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                await client.PostAsync("https://localhost:7219/api/Processor/Events_From_Generator", content);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при отправке события: {ex.Message}");
                throw;
            }
        }
    }
}
