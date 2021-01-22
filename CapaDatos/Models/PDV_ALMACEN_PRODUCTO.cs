using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("PDV_ALMACEN_PRODUCTO")]
    public partial class PDV_ALMACEN_PRODUCTO
    {
        [Key]
        [Column("alm_id")]
        public int AlmId { get; set; }
        [Column("pro_id")]
        public int ProId { get; set; }
        [Column("alp_stock_actual")]
        public double AlpStockActual { get; set; }
        [Column("alp_accion")]
        public Int16 AlpAccion { get; set; }
    }
}
