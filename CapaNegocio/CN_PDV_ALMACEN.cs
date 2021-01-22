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
    public class CN_PDV_ALMACEN
    {

        //Funcion que retorna los almacenes de omniventDemo
        private async Task<List<VM_PDV_ALMACEN>> ObtenerAlmacenes(int accion)
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    var almacenes = await context.PdvAlmacen.Select(
                                                            almacen => new VM_PDV_ALMACEN()
                                                            {
                                                                AlmId = almacen.AlmId,
                                                                AlmDescripcion = almacen.AlmDescripcion,
                                                                AlmEstatus = almacen.AlmEstatus,
                                                                SucId = almacen.SucId,
                                                                AlmAccion = almacen.AlmAccion
                                                            }
                                                        ).Where(
                                                            almacen => almacen.AlmAccion == accion
                                                        ).ToListAsync();

                    return almacenes;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public async Task<List<VM_PDV_ALMACEN>> ObtenerAlmacenesAInsertar() 
        {
            return await ObtenerAlmacenes(1);
        }

        public async Task<List<VM_PDV_ALMACEN>> ObtenerAlmacenesAActualizar() 
        {
            return await ObtenerAlmacenes(2);
        }

        public async Task<List<VM_PDV_ALMACEN>> ObtenerAlmacenesAEliminar() 
        {
            return await ObtenerAlmacenes(3);
        }

        private async Task DesincronizarDatos(List<VM_PDV_ALMACEN> almacenes)
        {
            using (var context = new OmniventContext())
            {
                var transaccion = await context.Database.BeginTransactionAsync();
                try
                {
                    foreach(var almacen in almacenes)
                    {
                        var dato = await context.PdvAlmacen.Where(a => a.AlmId == almacen.AlmId).FirstOrDefaultAsync();
                        if (dato != null)
                        {
                            dato.AlmAccion = 0;
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine("Dato no encontrado");
                        }
                    }

                    await transaccion.CommitAsync();
                    Console.WriteLine("El campo Accion de la tabla PDV_ALMACEN se ha actualizado correctamente");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    Console.WriteLine(ex);
                }
            }
        }

        
        public async Task InsertarAlmacenesAPI(List<VM_PDV_ALMACEN> almacenes, string token)
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
                    var response = await client.PostAsJsonAsync<List<VM_PDV_ALMACEN>>("Almacenes/Crear", almacenes);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(almacenes);
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

        public async Task ActualizarAlmacenesAPI(List<VM_PDV_ALMACEN> almacenes, string token)
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
                    var response = await client.PutAsJsonAsync<List<VM_PDV_ALMACEN>>("Almacenes/Actualizar", almacenes);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(almacenes);
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
    
        public async Task EliminarAlmacenesAPI(List<VM_PDV_ALMACEN> almacenes, string token)
        {
            try
            {
                var contador = 0;
                int[] ids = new int[almacenes.Count];
                almacenes.ForEach(x =>
                {
                    ids[contador] = x.AlmId;
                    contador++;
                });

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Helper.Helper.BASE_URI+ "Almacenes/Eliminar"),
                    Content = new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8, "application/json")
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
                        await DesincronizarDatos(almacenes);
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
