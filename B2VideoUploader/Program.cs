using B2VideoUploader.Helper;
using B2VideoUploader.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System.ComponentModel.Design;

namespace B2VideoUploader
{

    /**
     * Todo:
     * Fix retry mechanism
     * Only Reupload unuploaded parts
     * clean up half uploads
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
            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }

        public static IServiceProvider ServiceProvider { get; private set; }

        static IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                //read in password secrets
                configuration.AddIniFile("settings.ini", true);
            });
            return builder.ConfigureServices((context, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<Config>();
                    services.AddSingleton<CustomLogger>();
                    services.AddSingleton<CredentialConfigForm>();
                    services.AddSingleton<ConnectionSettingsValidator>();
                    services.AddSingleton<FfmpegVideoConversionService>();
                    services.AddSingleton<BlackBlazeB2Api>();
                    services.AddSingleton<BlackBlazeUploadService>();
                    services.AddSingleton<FfmpegVideoConversionService>();
                    services.AddTransient<MainForm>();
                })
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.AddDebug();

                });

        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

    }
}