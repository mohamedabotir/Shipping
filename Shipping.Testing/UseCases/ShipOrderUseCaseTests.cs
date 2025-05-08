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

        _shippingRepoMock
            .Setup(r => r.UpdateShippingStage((int)shippingOrder.Id, PurchaseOrderStage.BeingShipped))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(shippingOrder, default))
            .ReturnsAsync(1);

        var result = await _useCase.ShipOrder(command);

        Assert.IsTrue(result.IsSuccess);
        Assert.That(shippingOrder.PackageOrder.OrderStage, Is.EqualTo(PurchaseOrderStage.BeingShipped));
        _shippingRepoMock.Verify(r => r.UpdateShippingStage((int)shippingOrder.Id, PurchaseOrderStage.BeingShipped), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(shippingOrder, default), Times.Once);
    }

    [Test]
    public async Task ShipOrder_ShouldFail_WhenRepositoryFails()
    {
        var command = new StartShippingCommand("PO123");

        _shippingRepoMock
            .Setup(r => r.GetShippingOrderByPurchaseOrderNumber("PO123"))
            .ReturnsAsync(Result.Fail<ShippingOrder>("Not found"));

        var result = await _useCase.ShipOrder(command);

        Assert.IsTrue(result.IsFailure);
        Assert.That(result.Message, Is.EqualTo("Not found"));
    }

    [Test]
    public async Task ShipOrder_ShouldFail_WhenShippingOrderIsInInvalidStage()
    {
        var command = new StartShippingCommand("PO123");

        var shippingOrder = new ShippingOrder(Guid.NewGuid(), 1,
            User.CreateInstance("Jane", Address.CreateInstance("Another St11234").Value, "456").Value,
            new PackageOrder(Money.CreateInstance(150).Value, ActivationStatus.Active, "PO123", PurchaseOrderStage.Created, Guid.NewGuid()));

        _shippingRepoMock
            .Setup(r => r.GetShippingOrderByPurchaseOrderNumber("PO123"))
            .ReturnsAsync(Result.Ok(shippingOrder));

        var result = await _useCase.ShipOrder(command);

        Assert.IsTrue(result.IsFailure);
    }
}
