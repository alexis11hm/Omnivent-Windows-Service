using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDatos.Models.ViewModel
{
    public class VM_PDV_LISTAP_DETALLE
    {
        public int LipId { get; set; }
        public int ProId { get; set; }
        public decimal LipDetSinIva { get; set; }
        public decimal LipDetConIva { get; set; }
        public Int16 LpdAccion { get; set; }
    }
}
