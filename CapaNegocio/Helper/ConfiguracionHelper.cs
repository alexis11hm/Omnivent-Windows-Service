using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos.Models;
using Microsoft.EntityFrameworkCore;

namespace CapaNegocio.Helper
{
    public class ConfiguracionHelper
    {

        public async Task<List<LCL_PARAMETRO>> ObtenerParametrosAsync()
        {
            using (var context = new OmniventContext())
            {
                var parametros = await context.LclParametro.ToListAsync();
                if(parametros != null)
                {
                    return parametros;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
