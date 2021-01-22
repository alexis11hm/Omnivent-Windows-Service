using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("GLB_SUCURSAL")]
    public partial class GLB_SUCURSAL
    {
        [Key]
        [Column("suc_id")]
        public Int16 SucId { get; set; }
        [Column("suc_nombre")]
        public string SucNombre { get; set; }
        [Column("suc_accion")]
        public Int16 SucAccion { get; set; }
    }
}
