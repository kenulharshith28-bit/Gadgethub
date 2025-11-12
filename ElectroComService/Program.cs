using Microsoft.EntityFrameworkCore;
using ElectroComService.Data;
using ElectroComService.Models;

var builder = WebApplication.CreateBuilder(args);

// Get connection string from appsettings.json
var conn = builder.Configuration.GetConnectionString("cString");

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(conn));

// Register Repositories
builder.Services.AddScoped<ProductRepo>();

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
