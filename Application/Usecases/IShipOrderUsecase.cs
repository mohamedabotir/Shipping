using Application.Commands;
using Common.Result;

namespace Infrastructure.Consumer.Usecases;

public interface IShipOrderUsecase
{
    Task<Result> ShipOrder(StartShippingCommand command);
}