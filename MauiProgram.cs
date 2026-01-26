using JournalManagementSystem.Data;
using JournalManagementSystem.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace JournalManagementSystem
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
            builder.Services.AddMudServices();
            builder.Services.AddScoped<IJournalService, JournalService>();
            builder.Services.AddScoped<IAnalyticService, AnalyticService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddSingleton<UserSessionService>();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

#endif


            return builder.Build();
        }
    }
}

