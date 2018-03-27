using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Hub3c.Mentify.API
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        private static readonly Assembly Assembly = typeof(Program).GetTypeInfo().Assembly;

        private static readonly string AssemblyVersion =
            Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            Assembly.GetName().Version.ToString();

        private static readonly string Title = $"Hub3c Mentify v{AssemblyVersion}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.Title = Title;
            Console.WriteLine(Title);
            try
            {
                //ConfigureLogging();
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application host terminated unexpectedly", ex.Message);
            }
            finally
            {
                Log.Information("---Finish---");
                Log.CloseAndFlush();
            }

        }
        private static void ConfigureLogging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var loggingConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("loggingconfig.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"loggingconfig.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(loggingConfig)
                .CreateLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
        }
    }
}