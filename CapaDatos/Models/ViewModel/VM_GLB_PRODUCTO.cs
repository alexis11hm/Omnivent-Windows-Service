using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDatos.Models.ViewModel
{
    public class VM_GLB_PRODUCTO
    {
        public int ProId { get; set; }
        public string ProDescripcion { get; set; }
        public string ProCodigoBarras { get; set; }
        public string ProIdentificacion { get; set; }
        public string Familia { get; set; }
        public string SubFamilia { get; set; }
        public decimal ProPrecioGeneralIva { get; set; }
        public decimal ProCostoGeneralIva { get; set; }
        public Int16 ProAccion { get; set; }
    }
}
