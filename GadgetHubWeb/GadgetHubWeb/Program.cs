using GadgetHubWeb.Services;

var builder = WebApplication.CreateBuilder(args);


//Login

builder.Services.AddHttpClient("GadgetHubAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7282/"); // GadgetHub API URL
});

 


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<ApiClient>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CartService>();

builder.Services.AddSession();


var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // ✅ Required for Cart
app.MapRazorPages();
app.UseHttpsRedirection();
app.UseAuthorization();

// Lightweight proxy to call GadgetHubAPI from client-side JS
app.MapMethods("/api-proxy/{**path}", new[] { "DELETE" }, async (string path, IHttpClientFactory httpClientFactory, HttpRequest request, HttpResponse response) =>
{
    var client = httpClientFactory.CreateClient("GadgetHubAPI");
    var targetUri = new Uri(client.BaseAddress!, $"api/{path}");

    using var apiRequest = new HttpRequestMessage(HttpMethod.Delete, targetUri);
    var apiResponse = await client.SendAsync(apiRequest);

    response.StatusCode = (int)apiResponse.StatusCode;
    var content = await apiResponse.Content.ReadAsStringAsync();
    if (!string.IsNullOrEmpty(content))
    {
        await response.WriteAsync(content);
    }
});
app.Run();



