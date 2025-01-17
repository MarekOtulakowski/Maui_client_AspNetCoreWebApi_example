using MauiClientApp.Models;
using MauiClientApp.Services;
using Microsoft.EntityFrameworkCore;
using SQLite;

namespace MauiClientApp
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, ConstantValues.DbName);
            var sqliteConnection = new SQLiteAsyncConnection(dbPath);

            AuthService.InitializeDatabase(sqliteConnection);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Filename={dbPath}"));

            return builder.Build();
        }
    }
}
