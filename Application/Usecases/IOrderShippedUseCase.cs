using Application.Commands;
using Common.Result;

namespace Application.Usecases;

public interface IOrderShippedUseCase
{
    Task<Result> MarkDocumentAsShipped(OrderShippedCommand shippedCommand);
}