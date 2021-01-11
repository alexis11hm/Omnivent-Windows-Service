using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CapaDatos.Models
{
    [Table("PDV_FLUJO_EFECTIVO")]
    public partial class PDV_FLUJO_EFECTIVO
    {
        [Key]
        [Column("fle_id")]
        public int FleId { get; set; }
        [Column("fle_fecha")]
        public DateTime FleFecha { get; set; }
        [Column("fle_importe")]
        public decimal FleImporte { get; set; }
        [Column("fop_id")]
        public int FopId { get; set; }
        [Column("fle_tipo")]
        public char FleTipo { get; set; }
        [Column("fle_referencia")]
        public string? FleReferencia { get; set; }
        [Column("fle_observaciones")]
        public string? FleObservaciones { get; set; }
        [Column("caj_id")]
        public int CajId { get; set; }
        [Column("suc_id ")]
        public Int16? SucId { get; set; }
        [Column("cac_id")]
        public int? CacId { get; set; }
        [Column("fle_accion")]
        public Int16 FleAccion { get; set; }


    }
}
