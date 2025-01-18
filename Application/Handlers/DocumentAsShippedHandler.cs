using Application.Commands;
using Application.Usecases;
using Common.Result;
using MediatR;

namespace Application.Handlers;

public class DocumentAsShippedHandler(IOrderShippedUseCase orderShippedUseCase):IRequestHandler<OrderShippedCommand,Result>
{
    public async Task<Result> Handle(OrderShippedCommand request, CancellationToken cancellationToken)
    {
        return await orderShippedUseCase.MarkDocumentAsShipped(request);
    }
}