using Microsoft.Extensions.Logging;
using System.IO;
using CommunityToolkit.Maui;

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

            // 🔹 Register Database and ViewModel
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "beds.db3");

            // ✅ Register services correctly
            builder.Services.AddSingleton<BedDatabase>(); // Ensure Database is registered
            builder.Services.AddSingleton<MainViewModel>(); // Ensure ViewModel is registered
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<App>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
