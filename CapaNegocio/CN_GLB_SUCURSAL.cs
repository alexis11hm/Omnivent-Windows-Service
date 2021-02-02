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
    public class CN_GLB_SUCURSAL
    {

        //Funcion que retorna los productos de omniventDemo
        private async Task<List<VM_GLB_SUCURSAL>> ObtenerSucursales(int accion)
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    var sucursales = await context.GlbSucursal.Select(
                                                            sucursal => new VM_GLB_SUCURSAL()
                                                            {
                                                                SucId = sucursal.SucId,
                                                                SucNombre = sucursal.SucNombre,
                                                                SucAccion = Convert.ToInt32(sucursal.SucAccion)
                                                            }
                                                        ).Where(
                                                            sucursal => sucursal.SucAccion == accion
                                                        ).ToListAsync();

                    return sucursales;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public async Task<List<VM_GLB_SUCURSAL>> ObtenerSucursalesAInsertar() 
        {
            return await ObtenerSucursales(1);
        }

        public async Task<List<VM_GLB_SUCURSAL>> ObtenerSucursalesAActualizar() 
        {
            return await ObtenerSucursales(2);
        }

        public async Task<List<VM_GLB_SUCURSAL>> ObtenerSucursalesAEliminar() 
        {
            return await ObtenerSucursales(3);
        }

        private async Task DesincronizarDatos(List<VM_GLB_SUCURSAL> sucursales)
        {
            using (var context = new OmniventContext())
            {
                var transaccion = await context.Database.BeginTransactionAsync();
                try
                {
                    foreach(var sucursal in sucursales)
                    {
                        var dato = await context.GlbSucursal.Where(s => s.SucId == sucursal.SucId).FirstOrDefaultAsync();
                        if (dato != null)
                        {
                            dato.SucAccion = 0;
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine("Dato no encontrado");
                        }
                    }

                    await transaccion.CommitAsync();
                    Console.WriteLine("El campo Accion de la tabla GLB_SUCURSAL se ha actualizado correctamente");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    Console.WriteLine(ex);
                }
            }
        }

        
        public async Task<string> InsertarSucursalesAPI(List<VM_GLB_SUCURSAL> sucursales, string token)
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
                    var response = await client.PostAsJsonAsync<List<VM_GLB_SUCURSAL>>("Sucursales/Crear", sucursales);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(sucursales);
                        Console.WriteLine(response.StatusCode);
                        return response.StatusCode + " -> " + "La peticion se realizo correctamente";
                    }
                    else
                    {
                        return "WARNING: " + response.StatusCode + " -> " + "No fue posible realizar la peticion";
                        Console.WriteLine(response.StatusCode);
                        Console.WriteLine("No fue posible realizar la peticion");
                    }
                }
                clientHandler = null;
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message + " -> " + ex.InnerException.Message;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
            }
        }

        
        public async Task<string> ActualizarSucursalesAPI(List<VM_GLB_SUCURSAL> sucursales, string token)
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
                    var response = await client.PutAsJsonAsync<List<VM_GLB_SUCURSAL>>("Sucursales/Actualizar", sucursales);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(sucursales);
                        Console.WriteLine(response.StatusCode);
                        return response.StatusCode + " -> " + "La peticion se realizo correctamente";
                    }
                    else
                    {
                        return "WARNING: " + response.StatusCode + " -> " + "No fue posible realizar la peticion";
                        Console.WriteLine(response.StatusCode);
                        Console.WriteLine("No fue posible realizar la peticion");
                    }
                }
                clientHandler = null;
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message + " -> " + ex.InnerException.Message;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
            }
        }

        
        public async Task<string> EliminarSucursalesAPI(List<VM_GLB_SUCURSAL> sucursales, string token)
        {
            try
            {
                var contador = 0;
                Int16[] ids = new Int16[sucursales.Count];
                sucursales.ForEach(x =>
                {
                    ids[contador] = x.SucId;
                    contador++;
                });

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Helper.Helper.BASE_URI+ "Sucursales/Eliminar"),
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
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(sucursales);
                        Console.WriteLine(response.StatusCode);
                        return response.StatusCode + " -> " + "La peticion se realizo correctamente";
                    }
                    else
                    {
                        return "WARNING: " + response.StatusCode + " -> " + "No fue posible realizar la peticion";
                        Console.WriteLine(response.StatusCode);
                        Console.WriteLine("No fue posible realizar la peticion");
                    }
                }
                clientHandler = null;
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message + " -> " + ex.InnerException.Message;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
            }
        }
        

    }
}
