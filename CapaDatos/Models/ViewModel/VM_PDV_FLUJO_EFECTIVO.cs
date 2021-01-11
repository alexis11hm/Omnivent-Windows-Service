using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDatos.Models.ViewModel
{
    public class VM_PDV_FLUJO_EFECTIVO
    {

        public int FleId { get; set; }
        public DateTime FleFecha { get; set; }
        public decimal FleImporte { get; set; }
        public int FopId { get; set; }
        public char FleTipo { get; set; }
        public string? FleReferencia { get; set; }
        public string? FleObservaciones { get; set; }
        public string FleDescripcion { get; set; }
        public string? Sucursal { get; set; }
        public int? CacId { get; set; }
        public Int16 FleAccion { get; set; }
    }
}
