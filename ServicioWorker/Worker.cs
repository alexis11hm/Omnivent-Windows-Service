using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using CapaNegocio;
using CapaDatos.Models;
using System.Configuration;
using CapaDatos.Models.ViewModel;

namespace ServicioWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        //Definicion de variables de manera global en la clase
        private CN_PDV_VENTA venta;
        private List<PDV_VENTA> ventas;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Obteniendo el usuario de la API y la contraseña definidos en el archivo de configuracion 
            //Archivo de configuracion: App.config
            string usuario = ConfigurationManager.AppSettings["UsuarioAPI"].ToString();
            string password = ConfigurationManager.AppSettings["PasswordAPI"].ToString();

            //Inciamos sesion en la API y obtenemos el token
            string token = await CapaNegocio.Helper.Helper.IniciarSesion(usuario, password);

            //Se realiza un ciclo infinito, este se cancela hasta que se pasa el token de cancelacion
            //se cancela presionando: ctrl + c
            while (!stoppingToken.IsCancellationRequested)
            {
                
                //Obtenemos las ventas de la bd de omnivent
                var ventas = await venta.ObtenerVentasAsync();

                //Llamamos la API e Isertamos las ventas en la BD de la api

                //NOTA: La segunda vez que se realiza la peticion genera un error, ya que los datos
                //ya han sido registrados, falta manejar este problema
                if(ventas != null)
                {
                    await venta.InsertarVentasAPI(ventas,token);
                }

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                //Realizamos una retraso de 1 minuto, y comienza de nuevo el ciclo, es decir, realiza una peticion cada minuto
                await Task.Delay(60 * 1000, stoppingToken);
            }
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //Instanciamos el objeto para acceder a los metodos de obtencion de ventas de la bd de omnivent
            //e insercion a la API
            venta = new CN_PDV_VENTA();

            //Deshabilitadmos la validacion del certificado SSL
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                   (se, cert, chain, sslerror) =>
                   {
                       return true;
                   };

            return base.StartAsync(cancellationToken);
        }




        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        
    }
}
