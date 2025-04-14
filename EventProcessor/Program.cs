using EventProcessor;
using EventProcessor.db_context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// добавляем background сервис
builder.Services.AddSingleton<EventProcessingService>();
builder.Services.AddHostedService<EventProcessingService>(provider =>
                                                                provider?.GetService<EventProcessingService>() ??
                                                                throw new InvalidOperationException("EventProcessingService is not found"));
// Подключаем бд, в моём случае PostgreSQL
// Ссылка на подключение указана в конфигурационном файле
builder.Services.AddDbContext<IncidentContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string not found."))); ;


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
