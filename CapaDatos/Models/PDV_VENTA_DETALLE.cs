using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("PDV_VENTA_DETALLE")]
    public partial class PDV_VENTA_DETALLE
    {
        [Key]
        [Column("ved_id")]
        public int VedId { get; set; }
        [Column("vta_id")]
        public int VtaId { get; set; }
        [Column("pro_id")]
        public int ProId { get; set; }
        [Column("ved_precio_con_iva")]
        public decimal VedPrecioConIva { get; set; }
        [Column("ved_importe_descuento")]
        public decimal? VedImporteDescuento { get; set; }
        [Column("ved_cantidad")]
        public double VedCantidad { get; set; }
        [Column("ved_accion")]
        public Int16 VedAccion { get; set; }
    }
}
