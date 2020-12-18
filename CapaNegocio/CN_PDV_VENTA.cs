using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CapaDatos;
using CapaDatos.Models;
using CapaDatos.Models.ViewModel;
using Microsoft.EntityFrameworkCore;


namespace CapaNegocio
{
    public class CN_PDV_VENTA
    {

        //Funcion que retorna las ventas de la base de datos de Omnivent
        public async Task<List<VM_PDV_VENTA>> ObtenerVentasAsync()
        {
            using (var context = new OmniventContext())
            {
                try
                {
                    //Obtenemos las ventas uniendo las tablas de vendedor, sucursal y precios
                    //Ya que necesitamos los nombres y no el ID de cada tabla
                    //Cuando ningún campo es nulo
                    var ventas = await context.PdvVenta.Join(
                                                        context.GlbSucursal,
                                                        venta => venta.SucId,
                                                        sucursal => sucursal.SucId,
                                                        (venta, sucursal) => new
                                                        {
                                                            Venta = venta,
                                                            Sucursal = sucursal
                                                        }
                                                        ).Join(
                                                        context.PdvVendedor,
                                                        mixVenta => mixVenta.Venta.VndId,
                                                        vendedor => vendedor.VndId,
                                                        (mixVenta, vendedor) => new
                                                        {
                                                            MixVenta = mixVenta,
                                                            Vendedor = vendedor
                                                        }
                                                        ).Join(
                                                        context.PdvListaPrecio,
                                                        mixMixVenta => mixMixVenta.MixVenta.Venta.LipId,
                                                        lista => lista.LipId,
                                                        (mixMixVenta, lista) => new
                                                        {
                                                            MixMixVenta = mixMixVenta,
                                                            Lista = lista
                                                        }).Select(x => new VM_PDV_VENTA()
                                                        {
                                                            VtaId = x.MixMixVenta.MixVenta.Venta.VtaId,
                                                            VtaFolioVenta = x.MixMixVenta.MixVenta.Venta.VtaFolioVenta,
                                                            VtaFecha = x.MixMixVenta.MixVenta.Venta.VtaFecha,
                                                            VtaTotal = x.MixMixVenta.MixVenta.Venta.VtaTotal,
                                                            VtaEstatus = x.MixMixVenta.MixVenta.Venta.VtaEstatus,
                                                            Sucursal = x.MixMixVenta.MixVenta.Sucursal.SucNombre,
                                                            ListaPrecios = x.Lista.LipNombre,
                                                            Vendedor = x.MixMixVenta.Vendedor.VndNombre,
                                                            VtaAccion = x.MixMixVenta.MixVenta.Venta.VtaAccion
                                                        }).Where(
                                                            venta => venta.VtaAccion == 1 && venta.Vendedor != null && venta.ListaPrecios != null)
                                                        .ToListAsync();

                    //Tanto vendedor como lista de precios son nulos
                    var ventasSucursal = await context.PdvVenta.Join(
                                                        context.GlbSucursal,
                                                        venta => venta.SucId,
                                                        sucursal => sucursal.SucId,
                                                        (venta, sucursal) => new
                                                        {
                                                            Venta = venta,
                                                            Sucursal = sucursal
                                                        }
                                                        ).Select(x => new VM_PDV_VENTA()
                                                        {
                                                            VtaId = x.Venta.VtaId,
                                                            VtaFolioVenta = x.Venta.VtaFolioVenta,
                                                            VtaFecha = x.Venta.VtaFecha,
                                                            VtaTotal = x.Venta.VtaTotal,
                                                            VtaEstatus = x.Venta.VtaEstatus,
                                                            Sucursal = x.Sucursal.SucNombre,
                                                            ListaPrecios = null,
                                                            Vendedor = null,
                                                            VtaAccion = x.Venta.VtaAccion
                                                        }).Where(
                                                            venta => venta.VtaAccion == 1 && venta.Vendedor == null && venta.ListaPrecios == null)
                                                        .ToListAsync();

                    //Solamente lista de precios es nula
                    var ventasListaPrecios = await context.PdvVenta.Join(
                                                        context.GlbSucursal,
                                                        venta => venta.SucId,
                                                        sucursal => sucursal.SucId,
                                                        (venta, sucursal) => new
                                                        {
                                                            Venta = venta,
                                                            Sucursal = sucursal
                                                        }
                                                        ).Join(
                                                        context.PdvVendedor,
                                                        venta => venta.Venta.VndId,
                                                        vendedor => vendedor.VndId,
                                                        (venta, vendedor) => new
                                                        {
                                                            Venta = venta,
                                                            Vendedor = vendedor
                                                        }
                                                        ).Select(x => new VM_PDV_VENTA()
                                                        {
                                                            VtaId = x.Venta.Venta.VtaId,
                                                            VtaFolioVenta = x.Venta.Venta.VtaFolioVenta,
                                                            VtaFecha = x.Venta.Venta.VtaFecha,
                                                            VtaTotal = x.Venta.Venta.VtaTotal,
                                                            VtaEstatus = x.Venta.Venta.VtaEstatus,
                                                            Sucursal = x.Venta.Sucursal.SucNombre,
                                                            ListaPrecios = null,
                                                            Vendedor = x.Vendedor.VndNombre,
                                                            VtaAccion = x.Venta.Venta.VtaAccion
                                                        }).Where(
                                                            venta => venta.VtaAccion == 1 && venta.Vendedor != null && venta.ListaPrecios == null)
                                                        .ToListAsync();

                    //Solamente el vendedor es nulo
                    var ventasListaVendedor = await context.PdvVenta.Join(
                                                        context.GlbSucursal,
                                                        venta => venta.SucId,
                                                        sucursal => sucursal.SucId,
                                                        (venta, sucursal) => new
                                                        {
                                                            Venta = venta,
                                                            Sucursal = sucursal
                                                        }
                                                        ).Join(
                                                        context.PdvListaPrecio,
                                                        venta => venta.Venta.LipId,
                                                        lista => lista.LipId,
                                                        (venta, lista) => new
                                                        {
                                                            Venta = venta,
                                                            Lista = lista
                                                        }).Select(x => new VM_PDV_VENTA()
                                                        {
                                                            VtaId = x.Venta.Venta.VtaId,
                                                            VtaFolioVenta = x.Venta.Venta.VtaFolioVenta,
                                                            VtaFecha = x.Venta.Venta.VtaFecha,
                                                            VtaTotal = x.Venta.Venta.VtaTotal,
                                                            VtaEstatus = x.Venta.Venta.VtaEstatus,
                                                            Sucursal = x.Venta.Sucursal.SucNombre,
                                                            ListaPrecios = x.Lista.LipNombre,
                                                            Vendedor = null,
                                                            VtaAccion = x.Venta.Venta.VtaAccion
                                                        }).Where(
                                                            venta => venta.VtaAccion == 1 && venta.Vendedor == null && venta.ListaPrecios != null)
                                                        .ToListAsync();
                    //Si hay ventas retornamos las ventas, caso contrario valor nulo

                    if(ventasSucursal.Count > 0) ventas.AddRange(ventasSucursal);
                    if (ventasListaVendedor.Count > 0) ventas.AddRange(ventasListaVendedor);
                    if (ventasListaPrecios.Count > 0) ventas.AddRange(ventasListaPrecios);

                    return ventas;
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    //Si ocurrio un error retornamos valor nulo
                    return null;
                }

            }
        }

        //Funcion que realiza la insercion en la APi de las ventas
        public async Task InsertarVentasAPI(List<VM_PDV_VENTA> ventas, string token)
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
                    var response = await client.PostAsJsonAsync<List<VM_PDV_VENTA>>("Ventas/Crear", ventas);

                    //Si la peticion se realizo de manera correcta obtemos un "OK" (200)
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
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
            }
        }
    }
}
