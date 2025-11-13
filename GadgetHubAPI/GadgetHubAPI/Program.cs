using Microsoft.EntityFrameworkCore;
using GadgetHubAPI.Data;
using GadgetHubAPI.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Configure HttpClient to ignore SSL certificate errors in development
if (builder.Environment.IsDevelopment())
{
    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
}

// Get connection string from appsettings.json
var conn = builder.Configuration.GetConnectionString("cString");

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conn));

// Register Repositories
builder.Services.AddScoped<ProductRepo>();
builder.Services.AddScoped<OrderRepo>();

// Register QuotationService with HttpClient
builder.Services.AddHttpClient<QuotationService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "GadgetHubAPI/1.0");
});


// Register ProductSyncService with HttpClient
builder.Services.AddHttpClient<ProductSyncService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    // Ignore SSL certificate errors in development
    client.DefaultRequestHeaders.Add("User-Agent", "GadgetHubAPI/1.0");
});

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



app.Run();
