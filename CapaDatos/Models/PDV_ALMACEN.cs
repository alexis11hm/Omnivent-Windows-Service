using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("PDV_ALMACEN")]
    public partial class PDV_ALMACEN
    {
        [Key]
        [Column("alm_id")]
        public int AlmId { get; set; }
        [Column("alm_descripcion")]
        public string AlmDescripcion { get; set; }
        [Column("alm_estatus")]
        public char AlmEstatus { get; set; }
        [Column("suc_id")]
        public Int16 SucId { get; set; }
        [Column("alm_accion")]
        public Int16 AlmAccion { get; set; }
    }
}
