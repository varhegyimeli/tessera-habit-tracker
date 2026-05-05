using Tessera.Core.Interfaces;
using Tessera.Core.Services;
using Tessera.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Use default property casing
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? $"Data Source={Path.Combine(AppContext.BaseDirectory, "tessera.db")}";

// Register data layer
builder.Services.AddTesseraData(connectionString);

// Register core services
builder.Services.AddScoped<IStreakCalculator, StreakCalculator>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

// Add CORS policy for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
