using CapaDatos.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Helper
{
    public class Helper
    {

        public static string BASE_URI = ConfigurationManager.AppSettings["Esquema"].ToString()+"://"+ ConfigurationManager.AppSettings["Servidor"]+":"+ ConfigurationManager.AppSettings["Puerto"]+"/api/";

        public static async Task<string> IniciarSesion(string usuario, string password)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                using (var client = new HttpClient(clientHandler))
                {
                    client.BaseAddress = new Uri(BASE_URI);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

                    Login login = new Login()
                    {
                        usuario = usuario,
                        password = password
                    };

                    var response = await client.PostAsJsonAsync<Login>("Usuarios/Login", login);

                    if (response.IsSuccessStatusCode)
                    {
                        var respuesta = response.Content.ReadAsAsync<Dictionary<string, string>>().Result;
                        clientHandler = null;
                        return respuesta["token"].ToString();
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode);
                        Console.WriteLine("No es posible iniciar sesion");
                        clientHandler = null;
                        return null;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
