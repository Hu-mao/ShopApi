using Shop.Domain.Models;

namespace Shop.Domain.Entities;

public class DeliveryAddress
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string House { get; set; } = null!;

    public string? Apartment { get; set; }

    public string? PostalCode { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}