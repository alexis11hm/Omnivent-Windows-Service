using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("GLB_FAMILIA")]
    public partial class GLB_FAMILIA
    {
        [Key]
        [Column("fam_id")]
        public int FamId { get; set; }
        [Column("fam_descripcion")]
        public string FamDescripcion { get; set; }
    }
}
