using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Salonfur.Core.Entities;

namespace Salonfur.Core.Data;

public static class Seeder
{
    public const string AdminRole = "Admin";

    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, SortOrder = 1, Name = "Wassen & föhnen", Tag = "alle honden", DurationMinutes = 60, PriceLabel = "prijs", Price = "op aanvraag",
                Description = "Zacht shampoo-bad, uitborstelen en droog föhnen. Fris en glanzend naar huis." },
            new Service { Id = 2, SortOrder = 2, Name = "Volledige trimbeurt", Tag = "meest gevraagd", DurationMinutes = 120, IsFeatured = true, PriceLabel = "prijs", Price = "op aanvraag",
                Description = "Wassen, knippen of scheren in model, nagels, oren en pootjes. Helemaal klaar." },
            new Service { Id = 3, SortOrder = 3, Name = "Plukken", Tag = "ruwharig", DurationMinutes = 90, PriceLabel = "prijs", Price = "op aanvraag",
                Description = "Handmatig plukken voor ruwharige rassen zoals teckels en terriërs. Behoudt de echte vachtstructuur." },
            new Service { Id = 4, SortOrder = 4, Name = "Nagels & oren", Tag = "tussendoor", DurationMinutes = 30, PriceLabel = "prijs", Price = "op aanvraag",
                Description = "Nagels knippen, oren reinigen en pootjes bijwerken. Kort langs, snel klaar." },
            new Service { Id = 5, SortOrder = 5, Name = "Puppy-kennismaking", Tag = "jonge honden", DurationMinutes = 60, PriceLabel = "prijs", Price = "op aanvraag",
                Description = "Rustig wennen aan de tafel, het water en de föhn. Een goede start voor later." },
            new Service { Id = 6, SortOrder = 6, Name = "Ontklitten", Tag = "extra", DurationMinutes = 15, IsBookable = false, PriceLabel = "per", Price = "kwartier",
                Description = "Voor vachten met klitten: geduldig uitkammen, of kort scheren als dat beter is voor je hond." });
    }

    // Called once at startup: the admin needs values from secrets and a hashed password, so it can't be HasData.
    public static async Task SeedAdminAsync(UserManager<AppUser> users, RoleManager<IdentityRole> roles, string email, string password)
    {
        if (!await roles.RoleExistsAsync(AdminRole))
            await roles.CreateAsync(new IdentityRole(AdminRole));

        var admin = await users.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new AppUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await users.CreateAsync(admin, password);
            if (!result.Succeeded)
                throw new InvalidOperationException("Admin seeding failed: " + string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        if (!await users.IsInRoleAsync(admin, AdminRole))
            await users.AddToRoleAsync(admin, AdminRole);
    }
}
