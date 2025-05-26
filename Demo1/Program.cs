using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SmartRide.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddSingleton<LocationTracker>();
builder.Services.AddSingleton<PaymentSimulator>();
builder.Services.AddSingleton<DataRepository>();
builder.Services.AddSingleton<BookingManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed errors during development
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Explicitly map the root URL to Ride/Index
app.MapGet("/", context =>
{
    context.Response.Redirect("/Ride");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Ride}/{action=Index}/{id?}");

app.Run();