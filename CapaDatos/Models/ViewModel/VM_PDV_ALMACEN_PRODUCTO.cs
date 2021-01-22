using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDatos.Models.ViewModel
{
    public class VM_PDV_ALMACEN_PRODUCTO
    {
        public int AlmId { get; set; }
        public int ProId { get; set; }
        public double AlpStockActual { get; set; }
        public Int16 AlpAccion { get; set; }
    }
}
