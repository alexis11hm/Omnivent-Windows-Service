using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using CapaNegocio;
using CapaNegocio.Helper;
using CapaDatos.Models;
using System.Configuration;
using CapaDatos.Models.ViewModel;

namespace ServicioWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        //Definicion de variables de manera global en la clase
        private HashHelper hash;

        private CN_PDV_VENTA venta;
        private CN_GLB_PRODUCTO producto;
        private CN_PDV_LISTA_PRECIO listaPrecio;
        private CN_PDV_LISTAP_DETALLE listaPrecioDetalles;
        private CN_PDV_VENTA_DETALLE ventaDetalle;
        private CN_PDV_FORMA_PAGO formaPago;
        private CN_PDV_FLUJO_EFECTIVO flujoEfectivo;

        private List<VM_PDV_VENTA> ventas;

        private List<VM_GLB_PRODUCTO> productosInsertar;
        private List<VM_GLB_PRODUCTO> productosActualizar;
        private List<VM_GLB_PRODUCTO> productosEliminar;

        private List<VM_PDV_LISTA_PRECIO> listaPreciosInsertar;
        private List<VM_PDV_LISTA_PRECIO> listaPreciosActualizar;
        private List<VM_PDV_LISTA_PRECIO> listaPreciosEliminar;

        private List<VM_PDV_LISTAP_DETALLE> listaPrecioDetallesInsertar;
        private List<VM_PDV_LISTAP_DETALLE> listaPrecioDetallesActualizar;
        private List<VM_PDV_LISTAP_DETALLE> listaPrecioDetallesEliminar;

        private List<VM_PDV_VENTA_DETALLE> ventasDetalleInsertar;
        private List<VM_PDV_VENTA_DETALLE> ventasDetalleActualizar;
        private List<VM_PDV_VENTA_DETALLE> ventasDetalleEliminar;

        private List<VM_PDV_FORMA_PAGO> formasPagoInsertar;
        private List<VM_PDV_FORMA_PAGO> formasPagoActualizar;
        private List<VM_PDV_FORMA_PAGO> formasPagoEliminar;

        private List<VM_PDV_FLUJO_EFECTIVO> flujosEfectivoInsertar;
        private List<VM_PDV_FLUJO_EFECTIVO> flujosEfectivoActualizar;
        private List<VM_PDV_FLUJO_EFECTIVO> flujosEfectivoEliminar;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Obteniendo el usuario de la API y la contrase�a definidos en el archivo de configuracion 
            //Archivo de configuracion: App.config
            string usuario = hash.Descifrar(ConfigurationManager.AppSettings["UsuarioAPI"].ToString());
            string password = hash.Descifrar(ConfigurationManager.AppSettings["PasswordAPI"].ToString());

            //Inciamos sesion en la API y obtenemos el token
            string token = await Helper.IniciarSesion(usuario, password);

            //Se realiza un ciclo infinito, este se cancela hasta que se pasa el token de cancelacion
            //se cancela presionando: ctrl + c
            while (!stoppingToken.IsCancellationRequested)
            {

                Console.WriteLine("\nIniciando la Carga de Datos a la API...\n");

                //Obtenemos los datos de la bd de omnivent

                ventas = await venta.ObtenerVentas();

                productosInsertar = await producto.ObtenerProductosAInsertar();
                productosActualizar = await producto.ObtenerProductosAActualizar();
                productosEliminar = await producto.ObtenerProductosAEliminar();

                listaPreciosInsertar = await listaPrecio.ObtenerListaPrecioAInsertar();
                listaPreciosActualizar = await listaPrecio.ObtenerListaPrecioAActualizar();
                listaPreciosEliminar = await listaPrecio.ObtenerListaPrecioAEliminar();

                listaPrecioDetallesInsertar = await listaPrecioDetalles.ObtenerListaPrecioDetalleAInsertar();
                listaPrecioDetallesActualizar = await listaPrecioDetalles.ObtenerListaPrecioDetalleAActualizar();
                listaPrecioDetallesEliminar = await listaPrecioDetalles.ObtenerListaPrecioDetalleAEliminar();

                ventasDetalleInsertar = await ventaDetalle.ObtenerVentasDetalleAInsertar();
                ventasDetalleActualizar = await ventaDetalle.ObtenerVentasDetalleAActualizar();
                ventasDetalleEliminar = await ventaDetalle.ObtenerVentasDetalleAEliminar();

                formasPagoInsertar = await formaPago.ObtenerFormasPagoAInsertar();
                formasPagoActualizar = await formaPago.ObtenerFormasPagoAActualizar();
                formasPagoEliminar = await formaPago.ObtenerFormasPagoAEliminar();

                flujosEfectivoInsertar = await flujoEfectivo.ObtenerFlujosEfectivoAInsertar();
                flujosEfectivoActualizar = await flujoEfectivo.ObtenerFlujosEfectivoAActualizar();
                flujosEfectivoEliminar = await flujoEfectivo.ObtenerFlujosEfectivoAEliminar();


                //Llamamos la API e Isertamos las ventas en la BD de la api

                //VENTAS

                if (ventas != null && ventas.Count > 0)
                {
                    await venta.InsertarVentasAPI(ventas,token);
                }
                else
                {
                    Console.WriteLine("No hay ventas nuevos, No es necesario realizar la peticion");
                }

                //PRODUCTOS

                if (productosInsertar != null && productosInsertar.Count > 0)
                {
                    await producto.InsertarProductosAPI(productosInsertar,token);
                }
                else
                {
                    Console.WriteLine("No hay productos nuevos, No es necesario realizar la peticion");
                }

                if (productosActualizar != null && productosActualizar.Count > 0)
                {
                    await producto.ActualizarProductosAPI(productosActualizar, token);
                }
                else
                {
                    Console.WriteLine("No hay productos para actualizar, No es necesario realizar la peticion");
                }

                if (productosEliminar != null && productosEliminar.Count > 0)
                {
                    await producto.EliminarProductosAPI(productosEliminar, token);
                }
                else
                {
                    Console.WriteLine("No hay productos para Eliminar, No es necesario realizar la peticion");
                }

                //LISTA PRECIOS
                
                if (listaPreciosInsertar != null && listaPreciosInsertar.Count > 0)
                {
                    await listaPrecio.InsertarListaPreciosAPI(listaPreciosInsertar,token);
                }
                else
                {
                    Console.WriteLine("No hay lista de precios nuevos, No es necesario realizar la peticion");
                }

                if (listaPreciosActualizar != null && listaPreciosActualizar.Count > 0)
                {
                    await listaPrecio.ActualizarListaPreciosAPI(listaPreciosActualizar, token);
                }
                else
                {
                    Console.WriteLine("No hay lista de precios a actualizar, No es necesario realizar la peticion");
                }

                if (listaPreciosEliminar != null && listaPreciosEliminar.Count > 0)
                {
                    await listaPrecio.EliminarListaPreciosAPI(listaPreciosEliminar, token);
                }
                else
                {
                    Console.WriteLine("No hay lista de precios a eliminar, No es necesario realizar la peticion");
                }

                //lISTA PRECIOS DETALLE
                
                if (listaPrecioDetallesInsertar != null && listaPrecioDetallesInsertar.Count > 0)
                {
                    await listaPrecioDetalles.InsertarListaPreciosDetalleAPI(listaPrecioDetallesInsertar, token);
                }
                else
                {
                    Console.WriteLine("No hay lista de precios detalle nuevos, No es necesario realizar la peticion");
                }
                if (listaPrecioDetallesActualizar != null && listaPrecioDetallesActualizar.Count > 0)
                {
                    await listaPrecioDetalles.ActualizarListaPreciosDetalleAPI(listaPrecioDetallesActualizar, token);
                }
                else
                {
                    Console.WriteLine("No hay lista de precios detalle para actualizar, No es necesario realizar la peticion");
                }
                if (listaPrecioDetallesEliminar != null && listaPrecioDetallesEliminar.Count > 0)
                {
                    await listaPrecioDetalles.EliminarListaPreciosDetalleAPI(listaPrecioDetallesEliminar, token);
                }
                else
                {
                    Console.WriteLine("No hay lista de precios detalle para eliminar, No es necesario realizar la peticion");
                }
               
                //Ventas Detalle

                if (ventasDetalleInsertar != null && ventasDetalleInsertar.Count > 0)
                {
                    await ventaDetalle.InsertarVentasDetalleAPI(ventasDetalleInsertar, token);
                }
                else
                {
                    Console.WriteLine("No hay ventas detalle nuevos, No es necesario realizar la peticion");
                }

                if (ventasDetalleActualizar != null && ventasDetalleActualizar.Count > 0)
                {
                    await ventaDetalle.ActualizarVentasDetalleAPI(ventasDetalleActualizar, token);
                }
                else
                {
                    Console.WriteLine("No hay ventas detalle por actualizar, No es necesario realizar la peticion");
                }

                if (ventasDetalleEliminar != null && ventasDetalleEliminar.Count > 0)
                {
                    await ventaDetalle.EliminarVentasDetalleAPI(ventasDetalleEliminar, token);
                }
                else
                {
                    Console.WriteLine("No hay ventas detalle por eliminar, No es necesario realizar la peticion");
                }

                //Formas Pago

                if (formasPagoInsertar != null && formasPagoInsertar.Count > 0)
                {
                    await formaPago.InsertarFormasPagoAPI(formasPagoInsertar, token);
                }
                else
                {
                    Console.WriteLine("No hay formas de pago nuevos, No es necesario realizar la peticion");
                }

                if (formasPagoActualizar != null && formasPagoActualizar.Count > 0)
                {
                    await formaPago.ActualizarFormasPagoAPI(formasPagoActualizar, token);
                }
                else
                {
                    Console.WriteLine("No hay formas de pago por eliminar, No es necesario realizar la peticion");
                }

                if (formasPagoEliminar != null && formasPagoEliminar.Count > 0)
                {
                    await formaPago.EliminarFormasPagoAPI(formasPagoEliminar, token);
                }
                else
                {
                    Console.WriteLine("No hay formas de pago por eliminar, No es necesario realizar la peticion");
                }

                //FLUJOS DE EFECTIVO

                if (flujosEfectivoInsertar != null && flujosEfectivoInsertar.Count > 0)
                {
                    await flujoEfectivo.InsertarFlujosEfectivoAPI(flujosEfectivoInsertar, token);
                }
                else
                {
                    Console.WriteLine("No hay flujos de efectivo nuevos, No es necesario realizar la peticion");
                }

                if (flujosEfectivoActualizar != null && flujosEfectivoActualizar.Count > 0)
                {
                    await flujoEfectivo.ActualizarFlujosEfectivoAPI(flujosEfectivoActualizar, token);
                }
                else
                {
                    Console.WriteLine("No hay flujos de efectivo para actualizar, No es necesario realizar la peticion");
                }

                if (flujosEfectivoEliminar != null && flujosEfectivoEliminar.Count > 0)
                {
                    await flujoEfectivo.EliminarFlujosEfectivoAPI(flujosEfectivoEliminar, token);
                }
                else
                {
                    Console.WriteLine("No hay flujos de efectivo por eliminar, No es necesario realizar la peticion");
                }
                 
                 

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);


                //Realizamos una retraso de 1 minuto, y comienza de nuevo el ciclo, es decir, realiza una peticion cada minuto
                await Task.Delay(5 * 60 * 1000, stoppingToken);
            }
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //Instanciamos el objeto para acceder a los metodos de obtencion de ventas de la bd de omnivent
            //e insercion a la API
            venta = new CN_PDV_VENTA();
            producto = new CN_GLB_PRODUCTO();
            listaPrecio = new CN_PDV_LISTA_PRECIO();
            listaPrecioDetalles = new CN_PDV_LISTAP_DETALLE();
            ventaDetalle = new CN_PDV_VENTA_DETALLE();
            formaPago = new CN_PDV_FORMA_PAGO();
            flujoEfectivo = new CN_PDV_FLUJO_EFECTIVO();

            hash = new HashHelper();

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
