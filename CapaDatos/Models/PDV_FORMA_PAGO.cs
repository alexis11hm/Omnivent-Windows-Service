using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CapaDatos.Models
{
    [Table("PDV_FORMA_PAGO")]
    public partial class PDV_FORMA_PAGO
    {
        [Key]
        [Column("fop_id")]
        public int FopId { get; set; }
        [Column("fop_descripcion")]
        public string FopDescripcion { get; set; }
        [Column("fop_accion")]
        public Int16 FopAccion { get; set; }

    }
}
