using Common.Events;
using Common.Result;

namespace Application.Usecases;

public interface IPlaceShipmentRequestUsecase
{
    Task<Result> CreateShipment(PurchaseOrderApproved request);
}