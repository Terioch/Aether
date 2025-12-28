using Aether.Core.Services;
using Aether.Extensions;
using Aether.Repositories;
using Aether.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.UseSerilog();
builder.AddCors();

var connections = builder.Configuration.GetSection("ConnectionStrings");

builder.Services.AddEndpointMappers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddAetherRepositories(connections["aether"]);

var port = Environment.GetEnvironmentVariable("PORT") ?? "7158";
builder.WebHost.UseUrls($"https://*:{port}");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseEndpointMappers();
app.UseCors();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();