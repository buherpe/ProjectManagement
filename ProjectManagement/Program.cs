using Microsoft.AspNetCore.Components.Authorization;
using ProjectManagement;
using Blazored.Modal;
using RazorClassLibrary;
using Microsoft.EntityFrameworkCore;
using Blazored.Toast;
using Serilog;
using Telegram.Bot;
using Newtonsoft.Json;
using TgHelper;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostBuilderContext, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(hostBuilderContext.Configuration));

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
builder.Services.AddBlazoredToast();
builder.Services.AddHostedService<ExchangeRateUpdater>();
//builder.Services.AddHostedService<TelegramBotWorker>();
builder.Services.AddTelegramBotClientWithWebhooks();
builder.Services.AddScoped<TgBotUpdateHandler>();
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

if (builder.Configuration.GetValue<bool>("TgBot:Enabled"))
{
    app.MapPost($"/{builder.Configuration["TgBot:Route"]}",
    async (HttpContext context, TgBotUpdateHandler handler,
        ILogger<Program> _logger, CancellationToken cancellationToken) =>
    {
        using var sr = new StreamReader(context.Request.Body);
        var str = await sr.ReadToEndAsync(cancellationToken);
        var update = JsonConvert.DeserializeObject<Telegram.Bot.Types.Update>(str);

        await handler.HandleUpdateAsync(update, cancellationToken);
    });
}

app.Run();
