﻿using CapaDatos.Models;
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
    public class CN_PDV_LISTA_PRECIO
    {

        //Funcion para obtener la lista de precios
        private async Task<List<VM_PDV_LISTA_PRECIO>> ObtenerListaPrecio(int accion)
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    return await context.PdvListaPrecio.Select(
                                x => new VM_PDV_LISTA_PRECIO()
                                {
                                    LipId = x.LipId,
                                    LipNombre = x.LipNombre,
                                    LipAccion = x.LipAccion
                                }
                                ).
                                Where(
                                x => x.LipAccion == accion
                                ).ToListAsync();
                    
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        public async Task<List<VM_PDV_LISTA_PRECIO>> ObtenerListaPrecioAInsertar()
        {
            return await ObtenerListaPrecio(1);
        }

        public async Task<List<VM_PDV_LISTA_PRECIO>> ObtenerListaPrecioAActualizar()
        {
            return await ObtenerListaPrecio(2);
        }

        public async Task<List<VM_PDV_LISTA_PRECIO>> ObtenerListaPrecioAEliminar()
        {
            return await ObtenerListaPrecio(3);
        }

        //Funcion para desincronizar datos
        private async Task DesincronizarDatos(List<VM_PDV_LISTA_PRECIO> listaPrecios)
        {
            using (var context = new OmniventContext())
            {
                var transaccion = await context.Database.BeginTransactionAsync();
                try
                {
                    foreach(var precios in listaPrecios)
                    {
                        var dato = await context.PdvListaPrecio.Where(precio => precio.LipId == precios.LipId).FirstOrDefaultAsync();
                        if (dato != null)
                        {
                            dato.LipAccion = 0;
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine("Dato no encontrado");
                        }
                    }

                    await transaccion.CommitAsync();
                    Console.WriteLine("El campo Accion de la tabla PDV_LISTA_PRECIO se ha actualizado correctamente");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    Console.WriteLine(ex.Message);
                }
            }
        }

        //Funciones para llamar API

        public async Task<string> InsertarListaPreciosAPI(List<VM_PDV_LISTA_PRECIO> listaPrecios, string token)
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
                    var response = await client.PostAsJsonAsync<List<VM_PDV_LISTA_PRECIO>>("ListaPrecios/Crear", listaPrecios);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(listaPrecios);
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

        public async Task<string> ActualizarListaPreciosAPI(List<VM_PDV_LISTA_PRECIO> listaPrecios, string token)
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
                    var response = await client.PutAsJsonAsync<List<VM_PDV_LISTA_PRECIO>>("ListaPrecios/Actualizar", listaPrecios);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(listaPrecios);
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

        public async Task<string> EliminarListaPreciosAPI(List<VM_PDV_LISTA_PRECIO> listaPrecios, string token)
        {
            try
            {
                var contador = 0;
                int[] ids = new int[listaPrecios.Count];
                listaPrecios.ForEach(x =>
                {
                    ids[contador] = x.LipId;
                    contador++;
                });

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Helper.Helper.BASE_URI + "ListaPrecios/Eliminar"),
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
                        await DesincronizarDatos(listaPrecios);
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
