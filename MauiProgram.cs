using Microsoft.Extensions.Logging;
using MobileApplicationDev.Services;

namespace MobileApplicationDev
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp(serviceProvider => new App(serviceProvider))
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<AddTermPage>();
            builder.Services.AddTransient<MainPage>();


            return builder.Build();
        }
    }
}


