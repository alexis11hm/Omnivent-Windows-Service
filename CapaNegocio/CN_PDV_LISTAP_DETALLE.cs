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
    public class CN_PDV_LISTAP_DETALLE
    {

        //Funcion para obtener la lista de precios detalle
        public async Task<List<VM_PDV_LISTAP_DETALLE>> ObtenerListaPrecioDetalle(int accion)
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    return await context.PdvListapDetalle.Select(
                                x => new VM_PDV_LISTAP_DETALLE()
                                {
                                    LipId = x.LipId,
                                    ProId = x.ProId,
                                    LipDetConIva = x.LpdPrecioConIva,
                                    LipDetSinIva = x.LpdPrecioSinIva,
                                    LpdAccion = x.LpdAccion
                                }
                                ).
                                Where(
                                x => x.LpdAccion == accion
                                ).ToListAsync();
                    
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        public async Task<List<VM_PDV_LISTAP_DETALLE>> ObtenerListaPrecioDetalleAInsertar()
        {
            return await ObtenerListaPrecioDetalle(1);
        }

        public async Task<List<VM_PDV_LISTAP_DETALLE>> ObtenerListaPrecioDetalleAActualizar()
        {
            return await ObtenerListaPrecioDetalle(2);
        }

        public async Task<List<VM_PDV_LISTAP_DETALLE>> ObtenerListaPrecioDetalleAEliminar()
        {
            return await ObtenerListaPrecioDetalle(3);
        }

        //Funcion para desincronizar datos
        public async Task DesincronizarDatos(List<VM_PDV_LISTAP_DETALLE> listaPreciosDetalle)
        {
            using (var context = new OmniventContext())
            {
                var transaccion = await context.Database.BeginTransactionAsync();
                try
                {


                    foreach (var preciosDetalle in listaPreciosDetalle)
                    {
                        var dato = await context.PdvListapDetalle.FromSqlRaw(
                                "select * from PDV_LISTAP_DETALLE where pro_id = "+preciosDetalle.ProId+" and lip_id = "+preciosDetalle.LipId
                                ).FirstOrDefaultAsync();

                        if (dato != null)
                        {
                            dato.LpdAccion = 0;
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine("Dato no encontrado");
                        }
                    }



                    await transaccion.CommitAsync();
                    Console.WriteLine("El campo Accion de la tabla PDV_LISTAP_DETALLE se ha actualizado correctamente");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    Console.WriteLine(ex.Message);
                }
            }
        }

        //Funciones para llamar API

        public async Task InsertarListaPreciosDetalleAPI(List<VM_PDV_LISTAP_DETALLE> listaPreciosDetalles, string token)
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
                    var response = await client.PostAsJsonAsync<List<VM_PDV_LISTAP_DETALLE>>("ListaPrecioDetalles/Crear", listaPreciosDetalles);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(listaPreciosDetalles);
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

        public async Task ActualizarListaPreciosDetalleAPI(List<VM_PDV_LISTAP_DETALLE> listaPreciosDetalles, string token)
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
                    var response = await client.PutAsJsonAsync<List<VM_PDV_LISTAP_DETALLE>>("ListaPrecioDetalles/Actualizar", listaPreciosDetalles);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(listaPreciosDetalles);
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


        public async Task EliminarListaPreciosDetalleAPI(List<VM_PDV_LISTAP_DETALLE> listaPreciosDetalle, string token)
        {
            try
            {
                
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Helper.Helper.BASE_URI + "ListaPrecioDetalles/Eliminar"),
                    Content = new StringContent(JsonConvert.SerializeObject(listaPreciosDetalle), Encoding.UTF8, "application/json")
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
                        await DesincronizarDatos(listaPreciosDetalle);
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
