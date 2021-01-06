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
    public class CN_GLB_PRODUCTO
    {

        //Funcion que retorna los productos de omniventDemo
        private async Task<List<VM_GLB_PRODUCTO>> ObtenerProdutos(int accion)
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    var productos = await context.GlbProducto.Join(
                                                        context.GlbFamilia,
                                                        producto => producto.FamId,
                                                        familia => familia.FamId,
                                                        (producto, familia) => new
                                                        {
                                                            Producto = producto,
                                                            Familia = familia
                                                        }
                                                        ).Select(
                                                            producto => new VM_GLB_PRODUCTO()
                                                            {
                                                                ProId = producto.Producto.ProId,
                                                                ProDescripcion = producto.Producto.ProDescripcion,
                                                                ProCodigoBarras = producto.Producto.ProCodigoBarras,
                                                                ProCostoGeneralIva = producto.Producto.ProCostoGeneralIva,
                                                                ProPrecioGeneralIva = producto.Producto.ProPrecioGeneralIva,
                                                                ProIdentificacion = producto.Producto.ProIdentificacion,
                                                                Familia = producto.Familia.FamDescripcion,
                                                                SubFamilia = producto.Producto.SubId.ToString(),
                                                                ProAccion = producto.Producto.ProAccion
                                                            }
                                                        ).Where(
                                                            producto => producto.ProAccion == accion
                                                        ).ToListAsync();

                    var subfamilias = await context.GlbSubfamilia.ToListAsync();

                    productos.ForEach(producto =>
                    {
                        producto.SubFamilia = subfamilias.Find(
                            x => x.SubId == Convert.ToInt16(producto.SubFamilia)).SubDescripcion.ToString();
                    }
                    );


                    return productos;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public async Task<List<VM_GLB_PRODUCTO>> ObtenerProductosAInsertar() 
        {
            return await ObtenerProdutos(1);
        }

        public async Task<List<VM_GLB_PRODUCTO>> ObtenerProductosAActualizar() 
        {
            return await ObtenerProdutos(2);
        }

        public async Task<List<VM_GLB_PRODUCTO>> ObtenerProductosAEliminar() 
        {
            return await ObtenerProdutos(3);
        }

        private async Task DesincronizarDatos(List<VM_GLB_PRODUCTO> productos)
        {
            using (var context = new OmniventContext())
            {
                var transaccion = context.Database.BeginTransaction();
                try
                {
                    productos.ForEach(producto => {
                        var dato = context.GlbProducto.Where(p => p.ProId == producto.ProId).FirstOrDefault();
                        if (dato != null)
                        {
                            dato.ProAccion = 0;
                            context.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("Dato no encontrado");
                        }
                    });
                    transaccion.Commit();
                    Console.WriteLine("El campo de los datos de GLB_PRODUCTO se han actualizado correctamente");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    Console.WriteLine(ex);
                }
            }
        }


        public async Task InsertarProductosAPI(List<VM_GLB_PRODUCTO> productos, string token)
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
                    var response = await client.PostAsJsonAsync<List<VM_GLB_PRODUCTO>>("Productos/Crear", productos);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(productos);
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

        public async Task ActualizarProductosAPI(List<VM_GLB_PRODUCTO> productos, string token)
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
                    var response = await client.PutAsJsonAsync<List<VM_GLB_PRODUCTO>>("Productos/Actualizar", productos);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(productos);
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

        public async Task EliminarProductosAPI(List<VM_GLB_PRODUCTO> productos, string token)
        {
            try
            {
                var contador = 0;
                int[] ids = new int[productos.Count];
                productos.ForEach(x =>
                {
                    ids[contador] = x.ProId;
                    contador++;
                });

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Helper.Helper.BASE_URI+ "Productos/Eliminar"),
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
                        await DesincronizarDatos(productos);
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
