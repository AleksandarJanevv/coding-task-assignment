using Claims;
using Claims.Auditing;
using Claims.Controllers;
using Claims.Exceptions;
using Claims.Services;
using Claims.Validation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

var builder = WebApplication.CreateBuilder(args);

// Required Services
builder.Services.AddControllers();

// SQL Server - Claims and Covers
builder.Services.AddDbContext<ClaimsRepository>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MongoDB - Auditing
var mongoClient = new MongoClient(
    builder.Configuration["MongoDb:ConnectionString"]);

var mongoDatabase = mongoClient.GetDatabase(
    builder.Configuration["MongoDb:DatabaseName"]);

builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

// Dependency Injection
builder.Services.AddScoped<IClaimsService, ClaimsService>();
builder.Services.AddScoped<ICoversService, CoversService>();
// Validators
builder.Services.AddScoped<ClaimsValidator>();
builder.Services.AddScoped<CoversValidator>();
builder.Services.AddSingleton<CoversServiceUtils>();

// DI - Auditing
builder.Services.AddSingleton<IAuditQueue, AuditQueue>();
builder.Services.AddScoped<IAuditer, Auditer>();
builder.Services.AddHostedService<AuditBackgroundService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ClaimsRepository>();

    context.Database.Migrate();
}

app.Run();

public partial class Program { }
