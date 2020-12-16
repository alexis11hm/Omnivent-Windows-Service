using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("PDV_VENDEDOR")]
    public partial class PDV_VENDEDOR
    {
        [Key]
        [Column("vnd_id")]
        public Int32 VndId { get; set; }
        [Column("vnd_nombre")]
        public string VndNombre { get; set; }
    }
}
