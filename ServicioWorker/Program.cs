using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using System.Configuration;

namespace ServicioWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
           //Ruta donde se encuentra el proyecto de servicio de windows en la maquina
           string ruta = ConfigurationManager.AppSettings["RutaLog"].ToString();

           //Logger configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                
                .WriteTo.File(ruta+"\\OmniventWindowsService\\ServicioWorker\\Logs\\LogFile_" + DateTimeOffset.Now.ToString("yyyy-M-d") + ".txt")
                .CreateLogger();

            try
            {
                Log.Information("Comenzando el servicio");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Hubo un problema al comenzar el servicio");
                return;
            }
            finally
            {
                //Si hay informacion el el buffer lo escribe en el archivo antes de terminar
                Log.CloseAndFlush();
            }

            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                })
                .UseSerilog();
    }
}
