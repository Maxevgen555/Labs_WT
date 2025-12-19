using Labs.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. ВСЕ регистрации сервисов ДО Build()
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("LabsDb"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// КОНФИГУРАЦИЯ CORS (КРИТИЧЕСКИ ВАЖНО для Blazor)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        policy =>
        {
            // Разрешаем запросы от адреса, на котором работает ваше Blazor-приложение
            policy.WithOrigins("https://localhost:5001", "https://localhost:7003", "http://localhost:5000")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// 2. ТОЛЬКО ПОСЛЕ ВСЕХ Add... вызываем Build()
var app = builder.Build();

// 3. Конфигурация конвейера запросов (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// АКТИВАЦИЯ политики CORS (должно стоять до UseAuthorization и MapControllers)
app.UseCors("AllowBlazorApp");

app.UseAuthorization();
app.MapControllers();

// Инициализация базы данных
await DbInitializer.SeedData(app);

app.Run();