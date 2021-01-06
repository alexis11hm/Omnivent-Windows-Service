using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("PDV_LISTAP_DETALLE")]
    public partial class PDV_LISTAP_DETALLE
    {
        
        [Column("lip_id")]
        public int LipId { get; set; }
        [Column("pro_id")]
        public int ProId { get; set; }
        [Column("lpd_precio_sin_iva", TypeName = "money")]
        public decimal LpdPrecioSinIva { get; set; }
        [Column("lpd_precio_con_iva", TypeName = "money")]
        public decimal LpdPrecioConIva { get; set; }
        [Column("lpd_accion")]
        public Int16 LpdAccion { get; set; }
    }
}
