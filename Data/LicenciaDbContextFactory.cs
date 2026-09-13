using LicenciaBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LicenciaBackend.Data;

public class LicenciaDbContextFactory : IDesignTimeDbContextFactory<LicenciaDbContext>
{
    public LicenciaDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<LicenciaDbContext>();
        var connectionString = config.GetConnectionString("Default");

        optionsBuilder.UseNpgsql(connectionString);

        return new LicenciaDbContext(optionsBuilder.Options);
    }
}
