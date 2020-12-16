using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapaDatos.Models
{
    [Table("PDV_LISTA_PRECIO")]
    public partial class PDV_LISTA_PRECIO
    {
        [Key]
        [Column("lip_id")]
        public int LipId { get; set; }
        [Column("lip_nombre")]
        public string LipNombre { get; set; }
    }
}
