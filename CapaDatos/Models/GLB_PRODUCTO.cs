using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("GLB_PRODUCTO")]
    public partial class GLB_PRODUCTO
    {
        [Key]
        [Column("pro_id")]
        public int ProId { get; set; }
        [Column("pro_descripcion")]
        public string ProDescripcion { get; set; }
        [Column("pro_codigo_barras")]
        public string ProCodigoBarras { get; set; }
        [Column("pro_identificacion")]
        public string ProIdentificacion { get; set; }
        [Column("fam_id")]
        public Int16 FamId { get; set; }
        [Column("sub_id")]
        public Int16 SubId { get; set; }
        [Column("pro_precio_general_iva", TypeName = "money")]
        public decimal ProPrecioGeneralIva { get; set; }
        [Column("pro_costo_general_iva", TypeName = "money")]
        public decimal ProCostoGeneralIva { get; set; }
        [Column("pro_accion")]
        public Int16 ProAccion { get; set; }
    }
}
