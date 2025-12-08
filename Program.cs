using JournalApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Configure pipeline
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// Применяем миграции при старте
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
    try
    {
        await context.Database.MigrateAsync(); // <-- Автоматически применяет миграции
        Console.WriteLine("Миграции применены");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка миграций: {ex.Message}");
    }
}

app.MapControllers();
app.Run();