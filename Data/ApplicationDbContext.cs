using Microsoft.EntityFrameworkCore;
using PapeleriaAPI.Models;

namespace PapeleriaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - Tablas de la base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetallesCompra { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<CorteCaja> CortesCaja { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== CONFIGURACIÓN DE USUARIO =====
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.IdUsuario).ValueGeneratedOnAdd();
            });

            // ===== CONFIGURACIÓN DE CATEGORIA =====
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias");
                entity.HasKey(e => e.IdCategoria);
                entity.Property(e => e.IdCategoria).ValueGeneratedOnAdd();
            });

            // ===== CONFIGURACIÓN DE PRODUCTO =====
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.IdProducto);
                entity.Property(e => e.IdProducto).ValueGeneratedOnAdd();

                // Relación con Categoria
                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.IdCategoria)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURACIÓN DE PROVEEDOR =====
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("Proveedores");
                entity.HasKey(e => e.IdProveedor);
                entity.Property(e => e.IdProveedor).ValueGeneratedOnAdd();
            });

            // ===== CONFIGURACIÓN DE COMPRA =====
            modelBuilder.Entity<Compra>(entity =>
            {
                entity.ToTable("Compras");
                entity.HasKey(e => e.IdCompra);
                entity.Property(e => e.IdCompra).ValueGeneratedOnAdd();

                // Relación con Proveedor
                entity.HasOne(c => c.Proveedor)
                      .WithMany()
                      .HasForeignKey(c => c.IdProveedor)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con Usuario
                entity.HasOne(c => c.Usuario)
                      .WithMany()
                      .HasForeignKey(c => c.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURACIÓN DE DETALLE COMPRA =====
            modelBuilder.Entity<DetalleCompra>(entity =>
            {
                entity.ToTable("DetallesCompra");
                entity.HasKey(e => e.IdDetalleCompra);
                entity.Property(e => e.IdDetalleCompra).ValueGeneratedOnAdd();

                // Relación con Compra
                entity.HasOne(dc => dc.Compra)
                      .WithMany(c => c.DetallesCompra)
                      .HasForeignKey(dc => dc.IdCompra)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con Producto
                entity.HasOne(dc => dc.Producto)
                      .WithMany()
                      .HasForeignKey(dc => dc.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURACIÓN DE VENTA =====
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("Ventas");
                entity.HasKey(e => e.IdVenta);
                entity.Property(e => e.IdVenta).ValueGeneratedOnAdd();

                // Relación con Usuario
                entity.HasOne(v => v.Usuario)
                      .WithMany()
                      .HasForeignKey(v => v.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURACIÓN DE DETALLE VENTA =====
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("DetallesVenta");
                entity.HasKey(e => e.IdDetalleVenta);
                entity.Property(e => e.IdDetalleVenta).ValueGeneratedOnAdd();

                // Relación con Venta
                entity.HasOne(dv => dv.Venta)
                      .WithMany(v => v.DetallesVenta)
                      .HasForeignKey(dv => dv.IdVenta)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con Producto
                entity.HasOne(dv => dv.Producto)
                      .WithMany()
                      .HasForeignKey(dv => dv.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURACIÓN DE CORTE CAJA =====
            modelBuilder.Entity<CorteCaja>(entity =>
            {
                entity.ToTable("CortesCaja");
                entity.HasKey(e => e.IdCorte);
                entity.Property(e => e.IdCorte).ValueGeneratedOnAdd();

                // Relación con Usuario
                entity.HasOne(cc => cc.Usuario)
                      .WithMany()
                      .HasForeignKey(cc => cc.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}