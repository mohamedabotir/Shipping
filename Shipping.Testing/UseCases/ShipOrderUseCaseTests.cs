using NUnit.Framework;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Usecases;
using Common.Result;
using Common.Repository;
using Domain.Entity;
using Domain.Repositories;
using Application.Commands;
using Common.Constants;
using Common.ValueObject;
using Domain.ValueObject;
using Common.Events;
namespace Usecases;

[TestFixture]
public class ShipOrderUseCaseTests
{
    private Mock<IShippingRepository> _shippingRepoMock;
    private Mock<IUnitOfWork<ShippingOrder>> _unitOfWorkMock;
    private Mock<IEventSourcing<ShippingOrder>> _eventSourcing;
    private ShipOrderUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _shippingRepoMock = new Mock<IShippingRepository>();
        _eventSourcing = new Mock<IEventSourcing<ShippingOrder>>();
        _unitOfWorkMock = new Mock<IUnitOfWork<ShippingOrder>>();
        _useCase = new ShipOrderUseCase(_eventSourcing.Object,_shippingRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Test]
    public async Task ShipOrder_ShouldSucceed_WhenOrderIsValid()
    {
        var command = new StartShippingCommand("PO123");

        var shippingOrder = new ShippingOrder(Guid.NewGuid(), 1,
            User.CreateInstance("John", Address.CreateInstance("Street 11234").Value, "123").Value,
            new PackageOrder(Money.CreateInstance(100).Value, ActivationStatus.Active, "PO123", PurchaseOrderStage.Approved, Guid.NewGuid()));

        _shippingRepoMock
            .Setup(r => r.GetShippingOrderByPurchaseOrderNumber("PO123"))
            .ReturnsAsync(Result.Ok(shippingOrder));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(shippingOrder, default))
            .ReturnsAsync(1);
        _eventSourcing
.Setup(x => x.GetByIdAsync(command.purchaseNumber, ""))
.ReturnsAsync(shippingOrder);

        _shippingRepoMock
            .Setup(r => r.GetShippingOrderByPurchaseOrderNumber("PO123"))
            .ReturnsAsync(Result.Fail<ShippingOrder>("Not found"));
        _shippingRepoMock
            .Setup(r => r.UpdateShippingStageByPurchaseNumber(shippingOrder.PackageOrder.PurchaseOrderNumber, shippingOrder.PackageOrder.OrderStage))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
                   .Setup(u => u.SaveChangesAsync(shippingOrder, default))
                   .ReturnsAsync(1);
        var result = await _useCase.ShipOrder(command);

        Assert.IsTrue(result.IsSuccess);
    }


}
