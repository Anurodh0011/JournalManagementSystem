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
            builder.Services.AddSingleton<IJournalService, JournalService>();
#endif

            return builder.Build();
        }
    }
}

