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
        public virtual DbSet<PDV_LISTA_PRECIO> PdvListaPrecio { get; set; }
        public virtual DbSet<LCL_PARAMETRO> LclParametro { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["omniventContext"].ToString());
                
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
