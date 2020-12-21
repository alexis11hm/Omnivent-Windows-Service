using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("LCL_PARAMETRO")]
    public partial class LCL_PARAMETRO
    {
        [Key]
        [Column("par_nombre")]
        public string ParNombre { get; set; }
        [Column("par_valor")]
        public string ParValor { get; set; }
    }
}
