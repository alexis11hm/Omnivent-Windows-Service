using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using System.Configuration; 

#nullable disable

namespace CapaDatos.Models
{
    public partial class OmniventContext : DbContext
    {
        public OmniventContext()
        {
        }

        public OmniventContext(DbContextOptions<OmniventContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PDV_VENTA> PdvVenta { get; set; }
        public virtual DbSet<GLB_SUCURSAL> GlbSucursal { get; set; }
        public virtual DbSet<PDV_VENDEDOR> PdvVendedor { get; set; }
        public virtual DbSet<LCL_PARAMETRO> LclParametro { get; set; }
        public virtual DbSet<GLB_PRODUCTO> GlbProducto { get; set; }
        public virtual DbSet<PDV_LISTA_PRECIO> PdvListaPrecio { get; set; }
        public virtual DbSet<PDV_LISTAP_DETALLE> PdvListapDetalle { get; set; }
        public virtual DbSet<GLB_FAMILIA> GlbFamilia { get; set; }
        public virtual DbSet<GLB_SUBFAMILIA> GlbSubfamilia { get; set; }
        public virtual DbSet<PDV_VENTA_DETALLE> PdvVentaDetalle { get; set; }
        public virtual DbSet<PDV_FORMA_PAGO> PdvFormaPago { get; set; }
        public virtual DbSet<PDV_FLUJO_EFECTIVO> PdvFlujoEfectivo { get; set; }
        public virtual DbSet<PDV_CAJA> PdvCaja { get; set; }
        public virtual DbSet<PDV_ALMACEN> PdvAlmacen { get; set; }
        public virtual DbSet<PDV_ALMACEN_PRODUCTO> PdvAlmacenProducto { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["omniventContext"].ToString());
                
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PDV_LISTAP_DETALLE>().HasKey(pd => new { pd.ProId, pd.LipId });
            modelBuilder.Entity<PDV_VENTA_DETALLE>().HasKey(vd => new { vd.VedId, vd.VtaId });
            modelBuilder.Entity<PDV_ALMACEN_PRODUCTO>().HasKey(ap => new { ap.ProId, ap.AlmId});
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
