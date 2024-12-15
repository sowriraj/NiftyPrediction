using Microsoft.EntityFrameworkCore;
using NifTyPredictor;
using System.Net;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ApplicationDb")); 
builder.Services.AddDbContext<ApplicationBankDbContext>(options => options.UseInMemoryDatabase("ApplicationBankDb"));
builder.Services.AddHttpClient();

// Register HttpClientFactory
// Register HttpClientFactory
// Register HttpClientFactory
// Register HttpClientFactory with System Default Proxy
var cookieContainer1 = new CookieContainer();
var cookieContainer2 = new CookieContainer();

builder.Services.AddHttpClient("nseClient", client =>
{
    client.BaseAddress = new Uri("https://www.nseindia.com/");
    client.DefaultRequestVersion = HttpVersion.Version11;
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
    client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
    client.DefaultRequestHeaders.Add("Referer", "https://www.nseindia.com/");
    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
    client.DefaultRequestHeaders.Add("Pragma", "no-cache");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-access-token");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    CookieContainer = cookieContainer1,
    UseProxy = false,
    DefaultProxyCredentials = CredentialCache.DefaultCredentials
});

builder.Services.AddHttpClient("nsebankClient", client =>
{
    client.BaseAddress = new Uri("https://www.nseindia.com/");
    client.DefaultRequestVersion = HttpVersion.Version11;
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
    client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
    client.DefaultRequestHeaders.Add("Referer", "https://www.nseindia.com/");
    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
    client.DefaultRequestHeaders.Add("Pragma", "no-cache");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-other-access-token");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    CookieContainer = cookieContainer2,
    UseProxy = false,
    DefaultProxyCredentials = CredentialCache.DefaultCredentials
});


var app = builder.Build();


// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
  app.UseExceptionHandler("/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  //app.UseHsts();
//}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
