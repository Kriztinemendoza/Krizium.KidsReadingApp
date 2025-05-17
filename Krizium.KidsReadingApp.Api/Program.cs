
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using Krizium.KidsReadingApp.Api.Middleware;
using Krizium.KidsReadingApp.Api.Services;
using Krizium.KidsReadingApp.Api.Services.Interfaces;
using Krizium.KidsReadingApp.Core.Interfaces;
using Krizium.KidsReadingApp.Data;
using Krizium.KidsReadingApp.Data.Repositories;

namespace Krizium.KidsReadingApp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/kidsreading_api-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Configure Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Kids Reading App API",
                    Version = "v1",
                    Description = "API for Kids Reading App providing books and reading progress functionality",
                    Contact = new OpenApiContact
                    {
                        Name = "Support",
                        Email = "support@kidsreadingapp.com"
                    }
                });

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // Add database context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Register repositories
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IReadingProgressRepository, ReadingProgressRepository>();

            // Register services
            builder.Services.AddScoped<IBookService, BookService>();
            //builder.Services.AddScoped<IReadingProgressService, ReadingProgressService>();

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMobileApp", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Add health checks
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // Add custom exception handling middleware
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("AllowMobileApp");

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/health");

            // Create database and seed data if in development
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var dbContext = services.GetRequiredService<AppDbContext>();

                    dbContext.Database.Migrate();

                    // Seed data if needed
                    //if (dbContext.Books == null || !dbContext.Books.Any())
                    //{
                    //    SeedData.Initialize(dbContext, null);
                    //}
                    var books = dbContext.Books;
                    if (books == null || !books.Any())
                    {
                        SeedData.Initialize(dbContext, null);
                    }
                }
            }

            app.Run();
        }
    }
}
