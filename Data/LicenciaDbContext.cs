using Microsoft.EntityFrameworkCore;
using LicenciaBackend.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LicenciaBackend.Data;

public class LicenciaDbContext : DbContext
{
    public LicenciaDbContext(DbContextOptions<LicenciaDbContext> options) : base(options) { }

    public DbSet<LicenciaRegistrada> Licencias => Set<LicenciaRegistrada>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LicenciaRegistrada>()
            .OwnsOne(l => l.Info);

        modelBuilder.Entity<LicenciaRegistrada>()
            .OwnsOne(l => l.Firma);
    }
}
