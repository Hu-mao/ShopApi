using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Infrastructure.Repositories
{
    public class DeliveryAddressRepository
    {
        public async Task<List<DeliveryAddress>> GetAllAsync(
    Guid userId,
    CancellationToken cancellationToken)
        {
            return await _context.DeliveryAddresses
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
