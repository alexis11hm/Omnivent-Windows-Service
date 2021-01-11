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
    public class CN_PDV_FORMA_PAGO
    {

        //Funcion que retorna los productos de omniventDemo
        private async Task<List<VM_PDV_FORMA_PAGO>> ObtenerFormasPago(int accion)
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    var formasPago = await context.PdvFormaPago.Select(
                                                            formaPago => new VM_PDV_FORMA_PAGO()
                                                            {
                                                                FopId = formaPago.FopId,
                                                                FopDescripcion = formaPago.FopDescripcion,
                                                                FopAccion = formaPago.FopAccion
                                                            }
                                                        ).Where(
                                                            formaPago => formaPago.FopAccion == accion
                                                        ).ToListAsync();
                    return formasPago;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public async Task<List<VM_PDV_FORMA_PAGO>> ObtenerFormasPagoAInsertar() 
        {
            return await ObtenerFormasPago(1);
        }

        public async Task<List<VM_PDV_FORMA_PAGO>> ObtenerFormasPagoAActualizar() 
        {
            return await ObtenerFormasPago(2);
        }

        public async Task<List<VM_PDV_FORMA_PAGO>> ObtenerFormasPagoAEliminar() 
        {
            return await ObtenerFormasPago(3);
        }

        private async Task DesincronizarDatos(List<VM_PDV_FORMA_PAGO> formasPago)
        {
            using (var context = new OmniventContext())
            {
                var transaccion = await context.Database.BeginTransactionAsync();
                try
                {
                    foreach(var formaPago in formasPago)
                    {
                        var dato = await context.PdvFormaPago.Where(f => f.FopId == formaPago.FopId).FirstOrDefaultAsync();
                        if (dato != null)
                        {
                            dato.FopAccion = 0;
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine("Dato no encontrado");
                        }
                    }

                    await transaccion.CommitAsync();
                    Console.WriteLine("El campo Accion de la tabla PDV_FORMA_PAGO se ha actualizado correctamente");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    Console.WriteLine(ex);
                }
            }
        }

        
        public async Task InsertarFormasPagoAPI(List<VM_PDV_FORMA_PAGO> formasPago, string token)
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
                    var response = await client.PostAsJsonAsync<List<VM_PDV_FORMA_PAGO>>("FormasPago/Crear", formasPago);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(formasPago);
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

        
        public async Task ActualizarFormasPagoAPI(List<VM_PDV_FORMA_PAGO> formasPago, string token)
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
                    var response = await client.PutAsJsonAsync<List<VM_PDV_FORMA_PAGO>>("FormasPago/Actualizar", formasPago);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(formasPago);
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

        public async Task EliminarFormasPagoAPI(List<VM_PDV_FORMA_PAGO> formasPago, string token)
        {
            try
            {
                var contador = 0;
                int[] ids = new int[formasPago.Count];
                formasPago.ForEach(x =>
                {
                    ids[contador] = x.FopId;
                    contador++;
                });

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Helper.Helper.BASE_URI+ "FormasPago/Eliminar"),
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
                        await DesincronizarDatos(formasPago);
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
