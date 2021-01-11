using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CapaDatos.Models
{
    [Table("PDV_CAJA")]
    public partial class PDV_CAJA
    {
        [Key]
        [Column("caj_id")]
        public int CajId { get; set; }
        [Column("caj_descripcion")]
        public string CajDescripcion { get; set; }

    }
}
