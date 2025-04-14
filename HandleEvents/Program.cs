using HandleEvents.EventGenerationService;
using HandleEvents.Generator;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Добавляем сервис для отправки http запросов
builder.Services.AddHttpClient();

// Добавим свой генератор
builder.Services.AddSingleton<IEventGenerator, EventGenerator>();

// Добавим background сервис
builder.Services.AddHostedService<EventGenerationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
