using Application.Commands;
using Common.Result;
using Infrastructure.Consumer.Usecases;
using MediatR;

namespace Application.Handlers;

public class StartShippingHandler(IShipOrderUsecase shipOrderUseCase):IRequestHandler<StartShippingCommand,Result>
{
    public async Task<Result> Handle(StartShippingCommand request, CancellationToken cancellationToken)
    {
        return await shipOrderUseCase.ShipOrder(request);
    }
}