using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDatos.Models.ViewModel
{
    public class VM_PDV_VENTA_DETALLE
    {

        public int VedId { get; set; }
        public int VtaId { get; set; }
        public int ProId { get; set; }
        public decimal VedPrecio { get; set; }
        public decimal? VedDescuento { get; set; }
        public double VedCantidad { get; set; }
        public Int16 VedAccion { get; set; }
    }
}
