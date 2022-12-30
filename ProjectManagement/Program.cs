using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using ProjectManagement;
using Blazored.Modal;
using RazorClassLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddDbContext<DbContext, MyContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Transient);
builder.Services.AddDbContextFactory<MyContext>(x =>
    x.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Transient);
//builder.Services.AddTransient<BaseFactory<ProjectManagement.Pages.Tasks.Task>,
//    ProjectManagement.Pages.Tasks.TaskFactory>();

builder.Services.AddScoped<WebsiteAuthenticator>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<WebsiteAuthenticator>());
builder.Services.AddBlazoredModal();
builder.Services.AddHostedService<ExchangeRateUpdater>();
builder.Services.AddHostedService<TelegramBotWorker>();
//builder.Services.AddHostedService<Q>();
builder.Services.AddDataProtection();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
