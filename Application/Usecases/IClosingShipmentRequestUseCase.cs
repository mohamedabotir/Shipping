using Common.Events;
using Common.Result;

namespace Application.Usecases;

public interface IClosingShipmentRequestUseCase
{
    Task<Result> CloseShipment(OrderClosed @event);
}