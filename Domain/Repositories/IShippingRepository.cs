using Common.Result;
using Domain.Entity;

namespace Domain.Repositories;

public interface IShippingRepository
{
    Task Save(ShippingOrder shippingOrder);
}