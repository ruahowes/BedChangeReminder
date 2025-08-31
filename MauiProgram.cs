using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BedChangeReminder.Services;
using BedChangeReminder.ViewModels;
using BedChangeReminder.Views.Pages;

namespace BedChangeReminder
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services for dependency injection
            builder.Services.AddSingleton<BedDatabase>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<AppShell>();

#if ANDROID
            builder.Services.AddSingleton<INotificationService, AndroidNotificationService>();
#else
            builder.Services.AddSingleton<INotificationService, DefaultNotificationService>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}