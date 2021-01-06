using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("GLB_SUBFAMILIA")]
    public partial class GLB_SUBFAMILIA
    {
        [Key]
        [Column("sub_id")]
        public Int16 SubId { get; set; }
        [Column("sub_descripcion")]
        public string SubDescripcion { get; set; }
    }
}
