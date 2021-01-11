using CapaDatos.Models;
using CapaDatos.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_PDV_VENTA_DETALLE
    {

        //Funcion que retorna los productos de omniventDemo
        private async Task<List<VM_PDV_VENTA_DETALLE>> ObtenerVentasDetalle(int accion)
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    var ventasDetalle = await context.PdvVentaDetalle.Select(
                                                            detalle => new VM_PDV_VENTA_DETALLE()
                                                            {
                                                                VedId = detalle.VedId,
                                                                VtaId = detalle.VtaId,
                                                                ProId = detalle.ProId,
                                                                VedPrecio = detalle.VedPrecioConIva,
                                                                VedDescuento = (detalle.VedImporteDescuento != null) ? detalle.VedImporteDescuento : Convert.ToDecimal(0.0),
                                                                VedCantidad = detalle.VedCantidad,
                                                                VedAccion = detalle.VedAccion
                                                            }
                                                        ).Where(
                                                            detalle => detalle.VedAccion == accion
                                                        ).ToListAsync();
                    return ventasDetalle;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public async Task<List<VM_PDV_VENTA_DETALLE>> ObtenerVentasDetalleAInsertar() 
        {
            return await ObtenerVentasDetalle(1);
        }

        public async Task<List<VM_PDV_VENTA_DETALLE>> ObtenerVentasDetalleAActualizar() 
        {
            return await ObtenerVentasDetalle(2);
        }

        public async Task<List<VM_PDV_VENTA_DETALLE>> ObtenerVentasDetalleAEliminar() 
        {
            return await ObtenerVentasDetalle(3);
        }

        private async Task DesincronizarDatos(List<VM_PDV_VENTA_DETALLE> ventasDetalle)
        {
            using (var context = new OmniventContext())
            {
                var transaccion = await context.Database.BeginTransactionAsync();
                try
                {
                    foreach(var detalle in ventasDetalle)
                    {
                        var dato = await context.PdvVentaDetalle.Where(
                                        d => d.VtaId == detalle.VtaId && d.VedId == detalle.VedId
                                        ).FirstOrDefaultAsync();
                        if (dato != null)
                        {
                            dato.VedAccion = 0;
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine("Dato no encontrado");
                        }
                    }

                    await transaccion.CommitAsync();
                    Console.WriteLine("El campo Accion de la tabla PDV_VENTA_DETALLE se ha actualizado correctamente");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    Console.WriteLine(ex);
                }
            }
        }
        

        public async Task InsertarVentasDetalleAPI(List<VM_PDV_VENTA_DETALLE> ventasDetalle, string token)
        {
            try
            {
                //Creamos un clientHandler para la validacion de certificado
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                //creamos un cliente para realizar las peticiones y le pasamos el ClientHandler
                using (var client = new HttpClient(clientHandler))
                {
                    //Agregamos la base de la ruta de la API 
                    client.BaseAddress = new Uri(Helper.Helper.BASE_URI);

                    //Agregamos encabezados, para enviar json y el token de autorizacion
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    //Realizamos la peticion, enviamos una lista de objetos de ventas en formato JSON
                    var response = await client.PostAsJsonAsync<List<VM_PDV_VENTA_DETALLE>>("VentaDetalles/Crear", ventasDetalle);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(ventasDetalle);
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode);
                        Console.WriteLine("No fue posible realizar la peticion");
                    }
                }
                clientHandler = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
            }
        }

        
        public async Task ActualizarVentasDetalleAPI(List<VM_PDV_VENTA_DETALLE> ventasDetalle, string token)
        {
            try
            {
                //Creamos un clientHandler para la validacion de certificado
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                //creamos un cliente para realizar las peticiones y le pasamos el ClientHandler
                using (var client = new HttpClient(clientHandler))
                {
                    //Agregamos la base de la ruta de la API 
                    client.BaseAddress = new Uri(Helper.Helper.BASE_URI);

                    //Agregamos encabezados, para enviar json y el token de autorizacion
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    //Realizamos la peticion, enviamos una lista de objetos de ventas en formato JSON
                    var response = await client.PutAsJsonAsync<List<VM_PDV_VENTA_DETALLE>>("VentaDetalles/Actualizar", ventasDetalle);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(ventasDetalle);
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode);
                        Console.WriteLine("No fue posible realizar la peticion");
                    }
                }
                clientHandler = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
            }
        }
        

        public async Task EliminarVentasDetalleAPI(List<VM_PDV_VENTA_DETALLE> ventasDetalle, string token)
        {
            try
            {
                
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Helper.Helper.BASE_URI+ "VentaDetalles/Eliminar"),
                    Content = new StringContent(JsonConvert.SerializeObject(ventasDetalle), Encoding.UTF8, "application/json")
                };

                //Creamos un clientHandler para la validacion de certificado
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                //creamos un cliente para realizar las peticiones y le pasamos el ClientHandler
                using (var client = new HttpClient(clientHandler))
                {
                    //Agregamos encabezados, para enviar json y el token de autorizacion
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    //Realizamos la peticion, enviamos una lista de objetos de ventas en formato JSON
                    var response = await client.SendAsync(request);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(ventasDetalle);
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode);
                        Console.WriteLine("No fue posible realizar la peticion");
                    }
                }
                clientHandler = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
            }
        }


    }
}
