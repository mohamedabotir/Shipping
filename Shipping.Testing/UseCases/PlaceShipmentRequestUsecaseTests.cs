using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Common.Result;
using Common.Repository;
using Domain.Repositories;
using Infrastructure.Consumer.Usecases;
using Common.ValueObject;
using Domain.Entity;
using Common.Constants;
using Common.Events;
namespace Usecases;

[TestFixture]
public class PlaceShipmentRequestUsecaseTests
{
    private Mock<IShippingRepository> _shippingRepoMock;
    private Mock<IUnitOfWork<ShippingOrder>> _unitOfWorkMock;
    private PlaceShipmentRequestUsecase _useCase;

    [SetUp]
    public void Setup()
    {
        _shippingRepoMock = new Mock<IShippingRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork<ShippingOrder>>();
        _useCase = new PlaceShipmentRequestUsecase(_shippingRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Test]
    public async Task CreateShipment_ShouldSucceed_WhenDataIsValid()
    {
        var request = new PurchaseOrderApproved(
            Guid.NewGuid(),
            "PO123",
            ActivationStatus.Active,
            Money.CreateInstance(300).Value,
            "John Doe",
            "123 Test St",
            "123456",
            PurchaseOrderStage.Approved
        );

        _shippingRepoMock.Setup(r => r.Save(It.IsAny<ShippingOrder>()))
                         .Returns(Task.CompletedTask);

        var result = await _useCase.CreateShipment(request);

        Assert.IsTrue(result.IsSuccess);
        _shippingRepoMock.Verify(r => r.Save(It.IsAny<ShippingOrder>()), Times.Once);
    }

    [Test]
    public async Task CreateShipment_ShouldFail_WhenAddressIsInvalid()
    {
        var request = new PurchaseOrderApproved(
            Guid.NewGuid(),
            "PO123",
            ActivationStatus.Active,
            Money.CreateInstance(300).Value,
            "John Doe",
            "", // Invalid address (empty)
            "123456",
            PurchaseOrderStage.Approved
        );

        var result = await _useCase.CreateShipment(request);

        Assert.IsTrue(result.IsFailure);
    }

}
