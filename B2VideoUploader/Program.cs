using B2VideoUploader.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;

namespace B2VideoUploader
{

    /**
     * Todo: todo
     * add ffmpeg support
     * add automatic json upload support
     * clean up half uploads
     * Remove secrets from code
     * upload to github
     * 
     */
    internal class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration. 
            var host = CreateHostBuilder().Build();
            ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            ServiceProvider = host.Services;
            Application.Run(ServiceProvider.GetRequiredService<Form1>());
        }

        public static IServiceProvider ServiceProvider { get; private set; }

        static IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                //read in password secrets
                configuration.AddIniFile("settings.ini");
            });
            return builder.ConfigureServices((context, services) =>
                {
                    services.AddSingleton<CustomLogger>();
                    services.AddSingleton<HttpClient>();
                    services.AddSingleton<BlackBlazeB2Api>();
                    services.AddSingleton<BlackBlazeUploadService>();
                    services.AddSingleton<FfmpegVideoConversionService>();
                    services.AddTransient<Form1>();
                })
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.AddDebug();

                });                ;

        }

    }
}