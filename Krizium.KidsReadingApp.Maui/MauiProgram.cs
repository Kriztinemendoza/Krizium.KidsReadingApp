using Microsoft.Extensions.Logging;
using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Data;
using Krizium.KidsReadingApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Krizium.KidsReadingApp.Maui.Services;
using Krizium.KidsReadingApp.Maui.Services.Logging;
using System.IO;

namespace Krizium.KidsReadingApp.Maui
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
                    // Add child-friendly fonts
                    fonts.AddFont("ComicNeue-Regular.ttf", "ComicNeue");
                    fonts.AddFont("ComicNeue-Bold.ttf", "ComicNeueBold");
                })
                .ConfigureEssentials(essentials =>
                {
                    // Configure essentials if needed
                });

            builder.Services.AddMauiBlazorWebView();

            // Configure logging
            SetupLogging(builder);

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Set up database context
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "kidsreading.db");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Register repositories - implementing Clean Architecture
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IReadingProgressRepository, ReadingProgressRepository>();

            // Register HttpClient for API communication
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var httpClient = new HttpClient();

                // Set default headers, timeout, etc.
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                // For a real app, you might want to add authentication headers here
                // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-token");

                return httpClient;
            });

            // Register API Service
            builder.Services.AddSingleton<IBookApiService, BookApiService>();

            // Register NetworkService for connectivity management
            builder.Services.AddSingleton<NetworkService>();

            // Register platform-specific services using interfaces 
            builder.Services.AddSingleton<ITtsService, MauiTtsService>();
            builder.Services.AddSingleton<IFileService, MauiFileService>();

            // Register application services
            builder.Services.AddSingleton<BookLibraryDataService>();
            builder.Services.AddSingleton<OfflineStorageService>();
            builder.Services.AddSingleton<ReadingProgressService>();
            builder.Services.AddSingleton<LogManager>();

            var app = builder.Build();

            // Initialize and seed database
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();

                // Seed initial data if needed
                //var seedData = //JsonSeedLoader.LoadSeedDataAsync().GetAwaiter().GetResult();
                SeedData.Initialize(dbContext, null);

                // Clean up old logs
                var logManager = scope.ServiceProvider.GetRequiredService<LogManager>();
                logManager.CleanupOldLogsAsync().Wait();
            }

            return app;
        }
       
        private static void SetupLogging(MauiAppBuilder builder)
        {
            // Create log directory if it doesn't exist
            var logDirectory = Path.Combine(FileSystem.AppDataDirectory, "Logs");
            Directory.CreateDirectory(logDirectory);

            // Current log file path
            var logFilePath = Path.Combine(logDirectory, $"app_{DateTime.Now:yyyyMMdd}.log");

#if DEBUG
            // In debug mode, log everything to console and file
            builder.Logging
                .AddDebug()
                .AddFileLogger(logFilePath, LogLevel.Debug);
#else
        // In release mode, only log warnings and errors to file
        builder.Logging
            .AddFileLogger(logFilePath, LogLevel.Warning);
#endif

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var loggerFactory = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger("MauiProgram");
                var exception = args.ExceptionObject as Exception;
                logger?.LogCritical(exception, "Unhandled application exception");
            };
        }

    }
}



