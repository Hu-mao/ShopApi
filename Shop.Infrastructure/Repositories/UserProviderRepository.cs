using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class UserProviderRepository(ShopDbContext _context) : IUserProviderRepository
{
    public async Task<Provider?> GetProviderAsync(string name)
    {
        return await _context.Providers
            .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<UserProvider?> GetAsync(Guid userId, int providerId)
    {
        return await _context.UserProviders
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.ProviderId == providerId);
    }

    public async Task<UserProvider> AddAsync(UserProvider userProvider)
    {
        userProvider.CreatedAt = DateTime.UtcNow;
        await _context.UserProviders.AddAsync(userProvider);
        await _context.SaveChangesAsync();
        return userProvider;
    }
}
