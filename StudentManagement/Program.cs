using StudentManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudentContext>(options =>
                    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => // Add this configuration
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//var forecast = Enumerable.Range(1, 5).Select(index =>
//    new WeatherForecast
//    (
//        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//        Random.Shared.Next(-20, 55),
//        summaries[Random.Shared.Next(summaries.Length)]
//    ))
//    .ToArray();
//return forecast;
//})
//.WithName("GetWeatherForecast");


//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
