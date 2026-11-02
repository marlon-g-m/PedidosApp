using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Models;

namespace Pedidos.Api.Data;

// Primary constructor (C# 12): los parámetros llegan por inyección de dependencias
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API: reglas del esquema sin ensuciar la entidad con atributos
        modelBuilder.Entity<Pedido>(e =>
        {
            e.Property(p => p.Cliente).HasMaxLength(100).IsRequired();
            e.Property(p => p.Total).HasPrecision(18, 2);
        });
    }
}