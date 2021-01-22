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
    public class CN_PDV_ALMACEN_PRODUCTO
    {

        //Funcion que retorna los almacenes de omniventDemo
        private async Task<List<VM_PDV_ALMACEN_PRODUCTO>> ObtenerAlmacenProductos(int accion)
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    var almacenProductos = await context.PdvAlmacenProducto.Select(
                                                            almacenProducto => new VM_PDV_ALMACEN_PRODUCTO()
                                                            {
                                                                AlmId = almacenProducto.AlmId,
                                                                AlpStockActual = almacenProducto.AlpStockActual,
                                                                ProId = almacenProducto.ProId,
                                                                AlpAccion = almacenProducto.AlpAccion
                                                            }
                                                        ).Where(
                                                            almacenProducto => almacenProducto.AlpAccion == accion
                                                        ).ToListAsync();
                    return almacenProductos;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public async Task<List<VM_PDV_ALMACEN_PRODUCTO>> ObtenerAlmacenProductosAInsertar() 
        {
            return await ObtenerAlmacenProductos(1);
        }

        public async Task<List<VM_PDV_ALMACEN_PRODUCTO>> ObtenerAlmacenProductosAActualizar() 
        {
            return await ObtenerAlmacenProductos(2);
        }

        public async Task<List<VM_PDV_ALMACEN_PRODUCTO>> ObtenerAlmacenProductosAEliminar() 
        {
            return await ObtenerAlmacenProductos(3);
        }

        private async Task DesincronizarDatos(List<VM_PDV_ALMACEN_PRODUCTO> almacenProductos)
        {
            using (var context = new OmniventContext())
            {
                var transaccion = await context.Database.BeginTransactionAsync();
                try
                {
                    foreach(var almacenProducto in almacenProductos)
                    {
                        var dato = await context.PdvAlmacenProducto.Where(ap => ap.AlmId == almacenProducto.AlmId && ap.ProId == almacenProducto.ProId).FirstOrDefaultAsync();
                        if (dato != null)
                        {
                            dato.AlpAccion = 0;
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine("Dato no encontrado");
                        }
                    }

                    await transaccion.CommitAsync();
                    Console.WriteLine("El campo Accion de la tabla PDV_ALMACEN_PRODUCTO se ha actualizado correctamente");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    Console.WriteLine(ex);
                }
            }
        }

        
        public async Task InsertarAlmacenProductosAPI(List<VM_PDV_ALMACEN_PRODUCTO> almacenProductos, string token)
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
                    var response = await client.PostAsJsonAsync<List<VM_PDV_ALMACEN_PRODUCTO>>("AlmacenesProductos/Crear", almacenProductos);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(almacenProductos);
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

        
        public async Task ActualizarAlmacenProductosAPI(List<VM_PDV_ALMACEN_PRODUCTO> almacenProductos, string token)
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
                    var response = await client.PutAsJsonAsync<List<VM_PDV_ALMACEN_PRODUCTO>>("AlmacenesProductos/Actualizar", almacenProductos);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(almacenProductos);
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
        
        
        public async Task EliminarAlmacenProductosAPI(List<VM_PDV_ALMACEN_PRODUCTO> almacenProductos, string token)
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Helper.Helper.BASE_URI+ "AlmacenesProductos/Eliminar"),
                    Content = new StringContent(JsonConvert.SerializeObject(almacenProductos), Encoding.UTF8, "application/json")
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
                        await DesincronizarDatos(almacenProductos);
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
