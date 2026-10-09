using System.Reflection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Anemette.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, FeltKryptering kryptering)
    : DbContext(options), IDataProtectionKeyContext
{
    public DbSet<Bog> Boeger => Set<Bog>();
    public DbSet<Ordre> Ordrer => Set<Ordre>();
    public DbSet<Ordrelinje> Ordrelinjer => Set<Ordrelinje>();
    public DbSet<Arrangement> Arrangementer => Set<Arrangement>();
    public DbSet<Tilmelding> Tilmeldinger => Set<Tilmelding>();
    public DbSet<Artikel> Artikler => Set<Artikel>();
    public DbSet<Side> Sider => Set<Side>();
    public DbSet<Kontaktbesked> Kontaktbeskeder => Set<Kontaktbesked>();
    public DbSet<Indstilling> Indstillinger => Set<Indstilling>();
    public DbSet<Administrator> Administratorer => Set<Administrator>();

    /// <summary>Nøgler til login-cookies og formularbeskyttelse, så de overlever genstart af appen.</summary>
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Bog>(e =>
        {
            e.ToTable("Boeger");
            e.HasIndex(b => b.Slug).IsUnique();
            e.Property(b => b.Pris).HasPrecision(10, 2);
        });
        mb.Entity<Ordre>(e =>
        {
            e.ToTable("Ordrer");
            e.HasMany(o => o.Linjer).WithOne(l => l.Ordre!).HasForeignKey(l => l.OrdreId).OnDelete(DeleteBehavior.Cascade);
            e.Ignore(o => o.Total);
        });
        mb.Entity<Ordrelinje>(e =>
        {
            e.ToTable("Ordrelinjer");
            e.Property(l => l.Stykpris).HasPrecision(10, 2);
            // Salgstal bevares, selv hvis bogen slettes
            e.HasOne(l => l.Bog).WithMany().HasForeignKey(l => l.BogId).OnDelete(DeleteBehavior.SetNull);
        });
        mb.Entity<Arrangement>(e =>
        {
            e.ToTable("Arrangementer");
            e.Property(a => a.Pris).HasPrecision(10, 2);
            e.HasMany(a => a.Tilmeldinger).WithOne(t => t.Arrangement!).HasForeignKey(t => t.ArrangementId).OnDelete(DeleteBehavior.Cascade);
        });
        mb.Entity<Tilmelding>().ToTable("Tilmeldinger");
        mb.Entity<Artikel>(e =>
        {
            e.ToTable("Artikler");
            e.HasIndex(a => a.Slug).IsUnique();
        });
        mb.Entity<Side>().ToTable("Sider");
        mb.Entity<Kontaktbesked>().ToTable("Kontaktbeskeder");
        mb.Entity<Indstilling>().ToTable("Indstillinger");
        mb.Entity<Administrator>(e =>
        {
            e.ToTable("Administratorer");
            e.HasIndex(a => a.Brugernavn).IsUnique();
        });
        mb.Entity<DataProtectionKey>().ToTable("DataProtectionKeys");

        // Alle egenskaber markeret [Krypteret] gemmes krypteret
        var konverter = new ValueConverter<string, string>(v => kryptering.Krypter(v), v => kryptering.Dekrypter(v));
        foreach (var entitet in mb.Model.GetEntityTypes())
        {
            foreach (var egenskab in entitet.ClrType.GetProperties().Where(p => p.GetCustomAttribute<KrypteretAttribute>() != null))
            {
                mb.Entity(entitet.ClrType).Property(egenskab.Name).HasConversion(konverter);
            }
        }
    }
}
