using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Free1X2.Shared.Services;
using Free1X2.Shared.Configuration;

namespace Free1X2.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog for logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/free1x2-api-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting Free1X2 Web API");
                
                var builder = WebApplication.CreateBuilder(args);
                
                ConfigureServices(builder.Services);
                
                var app = builder.Build();
                
                ConfigureMiddleware(app);
                
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            // Add controllers
            services.AddControllers();
            
            // Add API documentation
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Free1X2 API",
                    Version = "v0.77.1",
                    Description = "Web API backend for Free1X2 football pools analysis application",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Free1X2.com",
                        Url = new Uri("https://free1x2.com")
                    }
                });
            });
            
            // Add CORS for mobile applications
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMobileApps", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            
            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddSerilog();
            });
            
            // TODO: Register service implementations
            // services.AddScoped<IAnalysisService, AnalysisService>();
            // services.AddScoped<ITeamService, TeamService>();
            // services.AddScoped<IFilterService, FilterService>();
            // services.AddSingleton<IAppConfiguration, AppConfiguration>();
        }
        
        private static void ConfigureMiddleware(WebApplication app)
        {
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Free1X2 API v0.77.1");
                    c.RoutePrefix = string.Empty; // Serve Swagger UI at root
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowMobileApps");
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
