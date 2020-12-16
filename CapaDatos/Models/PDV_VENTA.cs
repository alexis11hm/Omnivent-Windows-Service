using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CapaDatos.Models
{
    [Table("PDV_VENTA")]
    [Index(nameof(VtaEstatus), Name = "IDX_VTA_ESTATUS")]
    [Index(nameof(VtaFecha), Name = "IDX_VTA_FECHAVENTA")]
    public partial class PDV_VENTA
    {
        [Key]
        [Column("vta_id")]
        public int VtaId { get; set; }
        [Column("vta_folio_venta")]
        public int VtaFolioVenta { get; set; }
        [Column("vta_fecha", TypeName = "datetime")]
        public DateTime VtaFecha { get; set; }
        [Column("vta_total", TypeName = "money")]
        public decimal VtaTotal { get; set; }
        [Required]
        [Column("vta_estatus")]
        public string VtaEstatus { get; set; }
        [Column("suc_id")]
        public short SucId { get; set; }
        [Column("vnd_id")]
        public int? VndId { get; set; }
        [Column("lip_id")]
        public int? LipId { get; set; }
        
    }
}
