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

        private InternetConnection conexionInternet;

        private CN_PDV_VENTA venta;
        private CN_GLB_PRODUCTO producto;
        private CN_PDV_LISTA_PRECIO listaPrecio;
        private CN_PDV_LISTAP_DETALLE listaPrecioDetalles;
        private CN_PDV_VENTA_DETALLE ventaDetalle;
        private CN_PDV_FORMA_PAGO formaPago;
        private CN_PDV_FLUJO_EFECTIVO flujoEfectivo;
        private CN_GLB_SUCURSAL sucursal;
        private CN_PDV_ALMACEN almacen;
        private CN_PDV_ALMACEN_PRODUCTO almacenProducto;

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

        private List<VM_GLB_SUCURSAL> sucursalesInsertar;
        private List<VM_GLB_SUCURSAL> sucursalesActualizar;
        private List<VM_GLB_SUCURSAL> sucursalesEliminar;

        private List<VM_PDV_ALMACEN> almacenesInsertar;
        private List<VM_PDV_ALMACEN> almacenesActualizar;
        private List<VM_PDV_ALMACEN> almacenesEliminar;

        private List<VM_PDV_ALMACEN_PRODUCTO> almacenProductosInsertar;
        private List<VM_PDV_ALMACEN_PRODUCTO> almacenProductosActualizar;
        private List<VM_PDV_ALMACEN_PRODUCTO> almacenProductosEliminar;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //Obteniendo tiempo de carga de informacion
            int tiempo = Convert.ToInt32(ConfigurationManager.AppSettings["TiempoCarga"].ToString());
            //Obteniendo el usuario de la API y la contrase�a definidos en el archivo de configuracion 
            //Archivo de configuracion: App.config
            string usuario = hash.Descifrar(ConfigurationManager.AppSettings["UsuarioAPI"].ToString());
            string password = hash.Descifrar(ConfigurationManager.AppSettings["PasswordAPI"].ToString());

            string mensaje = "";

            _logger.LogInformation("Servicio corriendo a las: {time}", DateTimeOffset.Now);

            //Se realiza un ciclo infinito, este se cancela hasta que se pasa el token de cancelacion
            //se cancela presionando: ctrl + c
            while (!stoppingToken.IsCancellationRequested)
            {

                if (conexionInternet.HayConexionAInternet())
                {

                    //Inciamos sesion en la API y obtenemos el token
                    string token = await Helper.IniciarSesion(usuario, password);
                    if (token == null || token.Equals(""))
                    {
                        Console.WriteLine("No se pudo Iniciar Sesión");
                        _logger.LogError("ERROR - Inicio de Sesion - No se pudo iniciar sesion - {time}", DateTimeOffset.Now);

                    }
                    else
                    {
                        Console.WriteLine("\nIniciando la Carga de Datos a la API...\n");
                        _logger.LogInformation("Iniciando la Carga de Datos a la API - {time}", DateTimeOffset.Now);

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

                        sucursalesInsertar = await sucursal.ObtenerSucursalesAInsertar();
                        sucursalesActualizar = await sucursal.ObtenerSucursalesAActualizar();
                        sucursalesEliminar = await sucursal.ObtenerSucursalesAEliminar();

                        almacenesInsertar = await almacen.ObtenerAlmacenesAInsertar();
                        almacenesActualizar = await almacen.ObtenerAlmacenesAActualizar();
                        almacenesEliminar = await almacen.ObtenerAlmacenesAEliminar();

                        almacenProductosInsertar = await almacenProducto.ObtenerAlmacenProductosAInsertar();
                        almacenProductosActualizar = await almacenProducto.ObtenerAlmacenProductosAActualizar();
                        almacenProductosEliminar = await almacenProducto.ObtenerAlmacenProductosAEliminar();


                        //Llamamos la API e Isertamos las ventas en la BD de la api


                        //VENTAS

                        if (ventas != null && ventas.Count > 0)
                        {
                            mensaje = await venta.InsertarVentasAPI(ventas, token);
                            _logger.LogInformation("VENTAS - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay ventas nuevas, No es necesario realizar la peticion");
                            _logger.LogInformation("VENTAS - " + "No hay ventas nuevas, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //PRODUCTOS

                        if (productosInsertar != null && productosInsertar.Count > 0)
                        {
                            mensaje = await producto.InsertarProductosAPI(productosInsertar, token);
                            _logger.LogInformation("PRODUCTOS - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay productos nuevos, No es necesario realizar la peticion");
                            _logger.LogInformation("PRODUCTOS - " + "No hay productos nuevos, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (productosActualizar != null && productosActualizar.Count > 0)
                        {
                            mensaje = await producto.ActualizarProductosAPI(productosActualizar, token);
                            _logger.LogInformation("PRODUCTOS - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay productos para actualizar, No es necesario realizar la peticion");
                            _logger.LogInformation("PRODUCTOS - " + "No hay productos para actualizar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (productosEliminar != null && productosEliminar.Count > 0)
                        {
                            mensaje = await producto.EliminarProductosAPI(productosEliminar, token);
                            _logger.LogInformation("PRODUCTOS - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay productos para Eliminar, No es necesario realizar la peticion");
                            _logger.LogInformation("PRODUCTOS - " + "No hay productos para Eliminar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //LISTA PRECIOS

                        if (listaPreciosInsertar != null && listaPreciosInsertar.Count > 0)
                        {
                            mensaje = await listaPrecio.InsertarListaPreciosAPI(listaPreciosInsertar, token);
                            _logger.LogInformation("LISTA_PRECIOS - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay lista de precios nuevos, No es necesario realizar la peticion");
                            _logger.LogInformation("LISTA_PRECIOS - " + "No hay lista de precios nuevos, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (listaPreciosActualizar != null && listaPreciosActualizar.Count > 0)
                        {
                            mensaje = await listaPrecio.ActualizarListaPreciosAPI(listaPreciosActualizar, token);
                            _logger.LogInformation("LISTA_PRECIOS - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay lista de precios a actualizar, No es necesario realizar la peticion");
                            _logger.LogInformation("LISTA_PRECIOS - " + "No hay lista de precios a actualizar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (listaPreciosEliminar != null && listaPreciosEliminar.Count > 0)
                        {
                            mensaje = await listaPrecio.EliminarListaPreciosAPI(listaPreciosEliminar, token);
                            _logger.LogInformation("LISTA_PRECIOS - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay lista de precios a eliminar, No es necesario realizar la peticion");
                            _logger.LogInformation("LISTA_PRECIOS - " + "No hay lista de precios a eliminar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //lISTA PRECIOS DETALLE

                        if (listaPrecioDetallesInsertar != null && listaPrecioDetallesInsertar.Count > 0)
                        {
                            mensaje = await listaPrecioDetalles.InsertarListaPreciosDetalleAPI(listaPrecioDetallesInsertar, token);
                            _logger.LogInformation("LISTA_PRECIOS_DETALLE - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay lista de precios detalle nuevos, No es necesario realizar la peticion");
                            _logger.LogInformation("LISTA_PRECIOS_DETALLE - " + "No hay lista de precios detalle nuevos, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }
                        if (listaPrecioDetallesActualizar != null && listaPrecioDetallesActualizar.Count > 0)
                        {
                            mensaje = await listaPrecioDetalles.ActualizarListaPreciosDetalleAPI(listaPrecioDetallesActualizar, token);
                            _logger.LogInformation("LISTA_PRECIOS_DETALLE - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay lista de precios detalle para actualizar, No es necesario realizar la peticion");
                            _logger.LogInformation("LISTA_PRECIOS_DETALLE - " + "No hay lista de precios detalle para actualizar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }
                        if (listaPrecioDetallesEliminar != null && listaPrecioDetallesEliminar.Count > 0)
                        {
                            mensaje = await listaPrecioDetalles.EliminarListaPreciosDetalleAPI(listaPrecioDetallesEliminar, token);
                            _logger.LogInformation("LISTA_PRECIOS_DETALLE - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay lista de precios detalle para eliminar, No es necesario realizar la peticion");
                            _logger.LogInformation("LISTA_PRECIOS_DETALLE - " + "No hay lista de precios detalle para eliminar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //Ventas Detalle

                        if (ventasDetalleInsertar != null && ventasDetalleInsertar.Count > 0)
                        {
                            mensaje = await ventaDetalle.InsertarVentasDetalleAPI(ventasDetalleInsertar, token);
                            Console.WriteLine(mensaje);
                            _logger.LogInformation("VENTAS_DETALLE - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay ventas detalle nuevos, No es necesario realizar la peticion");
                            _logger.LogInformation("VENTAS_DETALLE - " + "No hay ventas detalle nuevos, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (ventasDetalleActualizar != null && ventasDetalleActualizar.Count > 0)
                        {
                            mensaje = await ventaDetalle.ActualizarVentasDetalleAPI(ventasDetalleActualizar, token);
                            _logger.LogInformation("VENTAS_DETALLE - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay ventas detalle por actualizar, No es necesario realizar la peticion");
                            _logger.LogInformation("VENTAS_DETALLE - " + "No hay ventas detalle por actualizar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (ventasDetalleEliminar != null && ventasDetalleEliminar.Count > 0)
                        {
                            mensaje = await ventaDetalle.EliminarVentasDetalleAPI(ventasDetalleEliminar, token);
                            _logger.LogInformation("VENTAS_DETALLE - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay ventas detalle por eliminar, No es necesario realizar la peticion");
                            _logger.LogInformation("VENTAS_DETALLE - " + "No hay ventas detalle por eliminar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //Formas Pago

                        if (formasPagoInsertar != null && formasPagoInsertar.Count > 0)
                        {
                            mensaje = await formaPago.InsertarFormasPagoAPI(formasPagoInsertar, token);
                            _logger.LogInformation("FORMAS_PAGO - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay formas de pago nuevos, No es necesario realizar la peticion");
                            _logger.LogInformation("FORMAS_PAGO - " + "No hay formas de pago nuevos, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (formasPagoActualizar != null && formasPagoActualizar.Count > 0)
                        {
                            mensaje = await formaPago.ActualizarFormasPagoAPI(formasPagoActualizar, token);
                            _logger.LogInformation("FORMAS_PAGO - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay formas de pago por eliminar, No es necesario realizar la peticion");
                            _logger.LogInformation("FORMAS_PAGO - " + "No hay formas de pago por eliminar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (formasPagoEliminar != null && formasPagoEliminar.Count > 0)
                        {
                            mensaje = await formaPago.EliminarFormasPagoAPI(formasPagoEliminar, token);
                            _logger.LogInformation("FORMAS_PAGO - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay formas de pago por eliminar, No es necesario realizar la peticion");
                            _logger.LogInformation("FORMAS_PAGO - " + "No hay formas de pago por eliminar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //FLUJOS DE EFECTIVO

                        if (flujosEfectivoInsertar != null && flujosEfectivoInsertar.Count > 0)
                        {
                            mensaje = await flujoEfectivo.InsertarFlujosEfectivoAPI(flujosEfectivoInsertar, token);
                            _logger.LogInformation("FLUJOS_EFECTIVO - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay flujos de efectivo nuevos, No es necesario realizar la peticion");
                            _logger.LogInformation("FLUJOS_EFECTIVO - " + "No hay flujos de efectivo nuevos, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (flujosEfectivoActualizar != null && flujosEfectivoActualizar.Count > 0)
                        {
                            mensaje = await flujoEfectivo.ActualizarFlujosEfectivoAPI(flujosEfectivoActualizar, token);
                            _logger.LogInformation("FLUJOS_EFECTIVO - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay flujos de efectivo para actualizar, No es necesario realizar la peticion");
                            _logger.LogInformation("FLUJOS_EFECTIVO - " + "No hay flujos de efectivo para actualizar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (flujosEfectivoEliminar != null && flujosEfectivoEliminar.Count > 0)
                        {
                            mensaje = await flujoEfectivo.EliminarFlujosEfectivoAPI(flujosEfectivoEliminar, token);
                            _logger.LogInformation("FLUJOS_EFECTIVO - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay flujos de efectivo por eliminar, No es necesario realizar la peticion");
                            _logger.LogInformation("FLUJOS_EFECTIVO - " + "No hay flujos de efectivo por eliminar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //SUCURSALES

                        if (sucursalesInsertar != null && sucursalesInsertar.Count > 0)
                        {
                            mensaje = await sucursal.InsertarSucursalesAPI(sucursalesInsertar, token);
                            _logger.LogInformation("SUCURSALES - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay sucursales nuevas, No es necesario realizar la peticion");
                            _logger.LogInformation("SUCURSALES - " + "No hay sucursales nuevas, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (sucursalesActualizar != null && sucursalesActualizar.Count > 0)
                        {
                            mensaje = await sucursal.ActualizarSucursalesAPI(sucursalesActualizar, token);
                            _logger.LogInformation("SUCURSALES - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay sucursales por actualizar, No es necesario realizar la peticion");
                            _logger.LogInformation("SUCURSALES - " + "No hay sucursales por actualizar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (sucursalesEliminar != null && sucursalesEliminar.Count > 0)
                        {
                            mensaje = await sucursal.EliminarSucursalesAPI(sucursalesEliminar, token);
                            _logger.LogInformation("SUCURSALES - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay sucursales por eliminar, No es necesario realizar la peticion");
                            _logger.LogInformation("SUCURSALES - " + "No hay sucursales por eliminar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //ALMACENES 

                        if (almacenesInsertar != null && almacenesInsertar.Count > 0)
                        {
                            mensaje = await almacen.InsertarAlmacenesAPI(almacenesInsertar, token);
                            _logger.LogInformation("ALMACENES - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay almacenes nuevos, No es necesario realizar la peticion");
                            _logger.LogInformation("ALMACENES - " + "No hay almacenes nuevos, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (almacenesActualizar != null && almacenesActualizar.Count > 0)
                        {
                            mensaje = await almacen.ActualizarAlmacenesAPI(almacenesActualizar, token);
                            _logger.LogInformation("ALMACENES - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay almacenes por actualizar, No es necesario realizar la peticion");
                            _logger.LogInformation("ALMACENES - " + "No hay almacenes por actualizar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (almacenesEliminar != null && almacenesEliminar.Count > 0)
                        {
                            mensaje = await almacen.EliminarAlmacenesAPI(almacenesEliminar, token);
                            _logger.LogInformation("ALMACENES - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay almacenes nuevos, No es necesario realizar la peticion");
                            _logger.LogInformation("ALMACENES - " + "No hay almacenes nuevos, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //PRODUCTOS ALMACEN
                        if (almacenProductosInsertar != null && almacenProductosInsertar.Count > 0)
                        {
                            mensaje = await almacenProducto.InsertarAlmacenProductosAPI(almacenProductosInsertar, token);
                            _logger.LogInformation("PRODUCTOS_ALMACENES - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay almacen producto nuevos, No es necesario realizar la peticion");
                            _logger.LogInformation("PRODUCTOS_ALMACENES - " + "No hay almacen producto nuevos, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (almacenProductosActualizar != null && almacenProductosActualizar.Count > 0)
                        {
                            mensaje = await almacenProducto.ActualizarAlmacenProductosAPI(almacenProductosActualizar, token);
                            _logger.LogInformation("PRODUCTOS_ALMACENES - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay almacen producto por actualizar, No es necesario realizar la peticion");
                            _logger.LogInformation("PRODUCTOS_ALMACENES - " + "No hay almacen producto por actualizar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        if (almacenProductosEliminar != null && almacenProductosEliminar.Count > 0)
                        {
                            mensaje = await almacenProducto.EliminarAlmacenProductosAPI(almacenProductosEliminar, token);
                            _logger.LogInformation("PRODUCTOS_ALMACENES - " + mensaje + " - {time}", DateTimeOffset.Now);
                        }
                        else
                        {
                            Console.WriteLine("No hay almacen producto por eliminar, No es necesario realizar la peticion");
                            _logger.LogInformation("PRODUCTOS_ALMACENES - " + "No hay almacen producto por eliminar, No es necesario realizar la peticion - {time}", DateTimeOffset.Now);
                        }

                        //Realizamos una retraso n minutos, y comienza de nuevo el ciclo, es decir, realiza una peticion cada minuto
                        await Task.Delay(tiempo * 60 * 1000, stoppingToken);
                    }
                }
                else
                {
                    Console.WriteLine("No hay conexion a internet");
                    _logger.LogError("NO_INTERNET - ERROR - No hay conexion a internet - {time}", DateTimeOffset.Now);
                    //Realizamos una retraso de10 segundos, para comprobar si hay internet de nuevo
                    await Task.Delay(10 * 1000, stoppingToken);
                }
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
            sucursal = new CN_GLB_SUCURSAL();
            almacen = new CN_PDV_ALMACEN();
            almacenProducto = new CN_PDV_ALMACEN_PRODUCTO();

            hash = new HashHelper();

            conexionInternet = new InternetConnection();

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
