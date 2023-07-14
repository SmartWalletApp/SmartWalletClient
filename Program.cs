using System;
using System.Net.Http.Headers;
using SmartWalletClient.MVC.Service;

var builder = WebApplication.CreateBuilder(args);

// Configure appsettings.json
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var config = configBuilder.Build();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<CustomerService>();

// Add LogInService to DI
builder.Services.AddScoped<LogInService>(); // Use this line if you don't have an interface for LogInService
// or
// builder.Services.AddScoped<ILogInService, LogInService>(); // Use this line if you have an interface for LogInService

// Add Session services.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
/*
// Inject IConfiguration into HttpClient of LogInService
string apiUrl = builder.Environment.IsDevelopment()
    ? config["ApiUrl:Debug"]
    : config["ApiUrl:Release"];

builder.Services.AddHttpClient<LogInService>(client =>
{
    client.BaseAddress = new Uri(apiUrl);

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Ignore all certificate validation errors
    ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
});
string apiUrl = builder.Environment.IsDevelopment()
    ? config["ApiUrl:Debug"]
    : config["ApiUrl:Release"];

builder.Services.AddHttpClient<CustomerService>(client =>
{
    client.BaseAddress = new Uri(apiUrl);

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Ignore all certificate validation errors
    ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
});*/
#if DEBUG
    string apiUrl  = builder.Configuration["ApiUrl:Debug"];
#else
string apiUrl = builder.Configuration["ApiUrl:Release"];
#endif



Action<HttpClient> configureClient = client =>
{
    client.BaseAddress = new Uri(apiUrl);

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
};

builder.Services.AddHttpClient<LogInService>(configureClient)
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        // Ignore all certificate validation errors
        ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
    });

builder.Services.AddHttpClient<CustomerService>(configureClient);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Session for the app.
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogIn}/{action=LogIn}");

app.Run();
