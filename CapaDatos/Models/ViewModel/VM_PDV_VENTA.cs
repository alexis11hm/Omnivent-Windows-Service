using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDatos.Models.ViewModel
{
    public class VM_PDV_VENTA
    {

        public int VtaId { get; set; }
        public int VtaFolioVenta { get; set; }
        public DateTime VtaFecha { get; set; }
        public decimal VtaTotal { get; set; }
        public string VtaEstatus { get; set; }
        public string Sucursal { get; set; }
        public string Vendedor { get; set; }
        public string ListaPrecios { get; set; }
        public Int16 VtaAccion { get; set; }
    }
}
