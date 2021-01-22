using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDatos.Models.ViewModel
{
    public class VM_PDV_ALMACEN
    {
        public int AlmId { get; set; }
        public string AlmDescripcion { get; set; }
        public char AlmEstatus { get; set; }
        public Int16 SucId { get; set; }
        public Int16 AlmAccion { get; set; }
    }
}
