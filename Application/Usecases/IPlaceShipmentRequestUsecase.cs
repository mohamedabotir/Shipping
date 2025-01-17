using Common.Events;
using Common.Result;

namespace Infrastructure.Consumer.Usecases;

public interface IPlaceShipmentRequestUsecase
{
    Task<Result> CreateShipment(PurchaseOrderApproved request);
}