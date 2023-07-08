using System;
using System.Net.Http.Headers;
using VuelingExchangeManagerClient.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddHttpClient<LogInService>(client =>
{
    client.BaseAddress = new Uri("https://167.86.71.33:9000");

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Ignora todos los errores de validación de certificados
    ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
});




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
