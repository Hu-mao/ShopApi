using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IUserProviderRepository
{
    Task<Provider?> GetProviderAsync(string name);
    Task<UserProvider?> GetAsync(Guid userId, int providerId);
    Task<UserProvider> AddAsync(UserProvider userProvider);
}
