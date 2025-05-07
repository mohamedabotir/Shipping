using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Application.Usecases;
using Application.Commands;
using Common.Result;
using Common.Repository;
using Domain.Repositories;
using Domain.Entity;
using Common.Constants;
using Common.ValueObject;
using Domain.ValueObject;
using Common.Events;
namespace Usecases;

[TestFixture]
public class OrderShippedUseCaseTests
{
    private Mock<IEventSourcing<ShippingOrder>> _eventSourcingMock;
    private Mock<IShippingRepository> _shippingRepoMock;
    private Mock<IUnitOfWork<ShippingOrder>> _unitOfWorkMock;
    private OrderShippedUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _eventSourcingMock = new Mock<IEventSourcing<ShippingOrder>>();
        _shippingRepoMock = new Mock<IShippingRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork<ShippingOrder>>();
        _useCase = new OrderShippedUseCase(_eventSourcingMock.Object, _shippingRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Test]
    public async Task MarkDocumentAsShipped_ShouldSucceed_WhenStageIsBeingShipped()
    {
        var command = new OrderShippedCommand("PO123");
        var shippingOrder = new ShippingOrder(Guid.NewGuid(), 1,
            User.CreateInstance("John", Address.CreateInstance("Main St1234").Value, "123").Value,
            new PackageOrder(Money.CreateInstance(200).Value, ActivationStatus.Active, "PO123", PurchaseOrderStage.BeingShipped, Guid.NewGuid()));

        _eventSourcingMock
            .Setup(x => x.GetByIdAsync("PO123",""))
            .ReturnsAsync(shippingOrder);

        _shippingRepoMock
            .Setup(r => r.UpdateShippingStage((int)shippingOrder.Id, PurchaseOrderStage.Shipped))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(shippingOrder, default))
            .ReturnsAsync(1);

        var result = await _useCase.MarkDocumentAsShipped(command);

        Assert.IsTrue(result.IsSuccess);
        Assert.That(shippingOrder.PackageOrder.OrderStage, Is.EqualTo(PurchaseOrderStage.Shipped));
        _shippingRepoMock.Verify(r => r.UpdateShippingStage((int)shippingOrder.Id, PurchaseOrderStage.Shipped), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(shippingOrder, default), Times.Once);
    }

    [Test]
    public async Task MarkDocumentAsShipped_ShouldFail_WhenOrderIsNotInBeingShippedStage()
    {
        var command = new OrderShippedCommand("PO123");
        var shippingOrder = new ShippingOrder(Guid.NewGuid(), 1,
            User.CreateInstance("Jane", Address.CreateInstance("Elm St1234456").Value, "456").Value,
            new PackageOrder(Money.CreateInstance(200).Value, ActivationStatus.Active, "PO123", PurchaseOrderStage.Approved, Guid.NewGuid()));

        _eventSourcingMock
            .Setup(x => x.GetByIdAsync("PO123", ""))
            .ReturnsAsync(shippingOrder);

        var result = await _useCase.MarkDocumentAsShipped(command);

        Assert.IsTrue(result.IsFailure);
    }
}
