using Common.Result;
using MediatR;

namespace Application.Commands;

public record OrderShippedCommand(string purchaseNumber) : IRequest<Result>;
