using Common.Result;
using MediatR;

namespace Application.Commands;

public record StartShippingCommand(string purchaseNumber) : IRequest<Result>;
