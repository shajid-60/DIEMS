using System;
using DIEMS.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register Oracle DB helper and repositories
builder.Services.AddScoped<OracleDbHelper>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<DisasterRepository>();
builder.Services.AddScoped<VictimRepository>();
builder.Services.AddScoped<ResourceRepository>();
builder.Services.AddScoped<ShelterRepository>();
builder.Services.AddScoped<HospitalRepository>();
builder.Services.AddScoped<VolunteerRepository>();
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddScoped<AnalyticsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable sessions in pipeline
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
