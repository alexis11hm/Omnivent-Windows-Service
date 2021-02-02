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
    public class CN_PDV_FLUJO_EFECTIVO
    {

        //Funcion que retorna los productos de omniventDemo
        private async Task<List<VM_PDV_FLUJO_EFECTIVO>> ObtenerFlujosEfectivo(int accion)
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    //Tabla Flujo efectivo con sucursal no nula
                    var flujoEfectivo = await context.PdvFlujoEfectivo.Join(
                                                        context.GlbSucursal,
                                                        flujoEfectivo => flujoEfectivo.SucId,
                                                        sucursal => sucursal.SucId,
                                                        (flujoEfectivo, sucursal) => new
                                                        {
                                                            flujoEfectivo = flujoEfectivo,
                                                            Sucursal = sucursal
                                                        }
                                                        ).Join(
                                                        context.PdvCaja,
                                                        flujoEfectivoMix => flujoEfectivoMix.flujoEfectivo.CajId,
                                                        caja => caja.CajId,
                                                        (flujoEfectivoMix,caja) => new
                                                        {
                                                            flujoEfectivoMix = flujoEfectivoMix,
                                                            Caja = caja
                                                        }
                                                        ).Select(
                                                            efectivo => new VM_PDV_FLUJO_EFECTIVO()
                                                            {
                                                                FleId = efectivo.flujoEfectivoMix.flujoEfectivo.FleId,
                                                                FleFecha = efectivo.flujoEfectivoMix.flujoEfectivo.FleFecha,
                                                                FleImporte = efectivo.flujoEfectivoMix.flujoEfectivo.FleImporte,
                                                                FleTipo = efectivo.flujoEfectivoMix.flujoEfectivo.FleTipo,
                                                                FleObservaciones = (efectivo.flujoEfectivoMix.flujoEfectivo.FleObservaciones != null) ? efectivo.flujoEfectivoMix.flujoEfectivo.FleObservaciones : "",
                                                                FleReferencia = (efectivo.flujoEfectivoMix.flujoEfectivo.FleReferencia != null) ? efectivo.flujoEfectivoMix.flujoEfectivo.FleReferencia : "",
                                                                FopId = efectivo.flujoEfectivoMix.flujoEfectivo.FopId,
                                                                FleDescripcion = efectivo.Caja.CajDescripcion,
                                                                CacId = (efectivo.flujoEfectivoMix.flujoEfectivo.CacId != null) ? efectivo.flujoEfectivoMix.flujoEfectivo.CacId : 0,
                                                                Sucursal = (efectivo.flujoEfectivoMix.Sucursal.SucNombre != null) ? efectivo.flujoEfectivoMix.Sucursal.SucNombre : "",
                                                                FleAccion = efectivo.flujoEfectivoMix.flujoEfectivo.FleAccion
                                                            }
                                                        ).Where(
                                                            efectivo => efectivo.FleAccion == accion
                                                            && efectivo.Sucursal != null
                                                        ).ToListAsync();


                    //Tabla Flujo efectivo con sucursal nula
                    var flujoEfectivoSucursal = await context.PdvFlujoEfectivo.Join(
                                                        context.PdvCaja,
                                                        flujoEfectivo => flujoEfectivo.CajId,
                                                        caja => caja.CajId,
                                                        (flujoEfectivo, caja) => new
                                                        {
                                                            FlujoEfectivo = flujoEfectivo,
                                                            Caja = caja
                                                        }
                                                        ).Select(
                                                            efectivo => new VM_PDV_FLUJO_EFECTIVO()
                                                            {
                                                                FleId = efectivo.FlujoEfectivo.FleId,
                                                                FleFecha = efectivo.FlujoEfectivo.FleFecha,
                                                                FleImporte = efectivo.FlujoEfectivo.FleImporte,
                                                                FleTipo = efectivo.FlujoEfectivo.FleTipo,
                                                                FleObservaciones = (efectivo.FlujoEfectivo.FleObservaciones != null) ? efectivo.FlujoEfectivo.FleObservaciones : "",
                                                                FleReferencia = (efectivo.FlujoEfectivo.FleReferencia != null) ? efectivo.FlujoEfectivo.FleReferencia : "",
                                                                FopId = efectivo.FlujoEfectivo.FopId,
                                                                FleDescripcion = efectivo.Caja.CajDescripcion,
                                                                CacId = (efectivo.FlujoEfectivo.CacId != null) ? efectivo.FlujoEfectivo.CacId : 0,
                                                                Sucursal = efectivo.FlujoEfectivo.SucId.ToString(),
                                                                FleAccion = efectivo.FlujoEfectivo.FleAccion
                                                            }
                                                        ).Where(
                                                            efectivo => efectivo.FleAccion == accion
                                                            && efectivo.Sucursal == null
                                                        ).ToListAsync();

                    if (flujoEfectivoSucursal.Count > 0) flujoEfectivo.AddRange(flujoEfectivoSucursal);

                    return flujoEfectivo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public async Task<List<VM_PDV_FLUJO_EFECTIVO>> ObtenerFlujosEfectivoAInsertar() 
        {
            return await ObtenerFlujosEfectivo(1);
        }

        public async Task<List<VM_PDV_FLUJO_EFECTIVO>> ObtenerFlujosEfectivoAActualizar() 
        {
            return await ObtenerFlujosEfectivo(2);
        }

        public async Task<List<VM_PDV_FLUJO_EFECTIVO>> ObtenerFlujosEfectivoAEliminar() 
        {
            return await ObtenerFlujosEfectivo(3);
        }

        private async Task DesincronizarDatos(List<VM_PDV_FLUJO_EFECTIVO> flujosEfectivo)
        {
            using (var context = new OmniventContext())
            {
                var transaccion = await context.Database.BeginTransactionAsync();
                try
                {
                    foreach(var efectivo in flujosEfectivo)
                    {
                        var dato = await context.PdvFlujoEfectivo.Where(f => f.FleId == efectivo.FleId).FirstOrDefaultAsync();
                        if (dato != null)
                        {
                            dato.FleAccion = 0;
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine("Dato no encontrado");
                        }
                    }

                    await transaccion.CommitAsync();
                    Console.WriteLine("El campo Accion de la tabla PDV_FLUJO_EFECTIVO se ha actualizado correctamente");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    Console.WriteLine(ex);
                }
            }
        }

        
        public async Task<string> InsertarFlujosEfectivoAPI(List<VM_PDV_FLUJO_EFECTIVO> flujosEfectivo, string token)
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
                    var response = await client.PostAsJsonAsync<List<VM_PDV_FLUJO_EFECTIVO>>("FlujosEfectivo/Crear", flujosEfectivo);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(flujosEfectivo);
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

        
        public async Task<string> ActualizarFlujosEfectivoAPI(List<VM_PDV_FLUJO_EFECTIVO> flujosEfectivo, string token)
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
                    var response = await client.PutAsJsonAsync<List<VM_PDV_FLUJO_EFECTIVO>>("FlujosEfectivo/Actualizar", flujosEfectivo);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        //Regresamos el valor de accion a 0
                        await DesincronizarDatos(flujosEfectivo);
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

        public async Task<string> EliminarFlujosEfectivoAPI(List<VM_PDV_FLUJO_EFECTIVO> flujosEfectivo, string token)
        {
            try
            {
                var contador = 0;
                int[] ids = new int[flujosEfectivo.Count];
                flujosEfectivo.ForEach(x =>
                {
                    ids[contador] = x.FleId;
                    contador++;
                });

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Helper.Helper.BASE_URI+ "FlujosEfectivo/Eliminar"),
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
                        await DesincronizarDatos(flujosEfectivo);
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
