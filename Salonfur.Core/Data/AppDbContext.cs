using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Salonfur.Core.Entities;

namespace Salonfur.Core.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options), IDataProtectionKeyContext
{
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ClosedPeriod> ClosedPeriods => Set<ClosedPeriod>();

    // Keeps login cookies and form tokens valid across container restarts on Render.
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Reservation>(e =>
        {
            e.HasIndex(r => r.Date);
            e.Property(r => r.Size).HasConversion<string>().HasMaxLength(10);
            e.HasOne(r => r.Service).WithMany(s => s.Reservations).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClosedPeriod>().HasIndex(c => new { c.StartDate, c.EndDate });

        Seeder.Seed(modelBuilder);
    }
}
