using System;
using System.Collections.Generic;
using System.Net;

namespace CapaNegocio.Helper
{
    public class InternetConnection
    {

        public bool HayConexionAInternet()
        {
            System.Uri Url = new System.Uri("https://www.google.com/");

            System.Net.WebRequest WebRequest;
            WebRequest = System.Net.WebRequest.Create(Url);
            System.Net.WebResponse objetoResp;

            try
            {
                objetoResp = WebRequest.GetResponse();
                objetoResp.Close();
                WebRequest = null;
                return true;
            }
            catch (Exception e)
            {
                WebRequest = null;
                return false;
            }
            
        }

    }
}
