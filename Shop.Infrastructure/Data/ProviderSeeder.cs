using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models;

namespace Shop.Infrastructure.Data;

public static class ProviderSeeder
{
    public static async Task SeedAsync(ShopDbContext context)
    {
        if (await context.Providers.AnyAsync())
            return;

        context.Providers.AddRange(
            new Provider { Id = 1, Name = "google" },
            new Provider { Id = 2, Name = "fb" },
            new Provider { Id = 3, Name = "apple" }
        );

        await context.SaveChangesAsync();
    }
}
