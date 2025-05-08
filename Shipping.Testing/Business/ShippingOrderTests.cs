using NUnit.Framework;
using System;
using Domain.Entity;
using Common.ValueObject;
using Common.Constants;
using Common.Events;
using Common.Result;
using System.Linq;
using Domain.ValueObject;

namespace UnitTests
{
    [TestFixture]
    public class ShippingOrderTests
    {
        private User CreateTestUser() =>
            User.CreateInstance("John Doe", Address.CreateInstance("123 Main St").Value, "1234567890").Value;

        private PackageOrder CreateApprovedPackageOrder() =>
            new PackageOrder(Money.CreateInstance(100).Value, ActivationStatus.Active, "PO123", PurchaseOrderStage.Approved, Guid.NewGuid());

        private PackageOrder CreateBeingShippedOrder() =>
            new PackageOrder(Money.CreateInstance(100).Value, ActivationStatus.Active, "PO123", PurchaseOrderStage.BeingShipped, Guid.NewGuid());

        [Test]
        public void ShipOrder_ShouldSucceed_WhenOrderIsApproved()
        {
            var order = new ShippingOrder(Guid.NewGuid(), 1, CreateTestUser(), CreateApprovedPackageOrder());

            var result = order.ShipOrder();

            Assert.IsTrue(result.IsSuccess);
            Assert.That(order.PackageOrder.OrderStage, Is.EqualTo(PurchaseOrderStage.BeingShipped));
            Assert.That(order.GetUncommittedEvents().Any(), Is.True);
        }

        [Test]
        public void ShipOrder_ShouldFail_WhenOrderNotApproved()
        {
            var notApproved = new PackageOrder(Money.CreateInstance(100).Value, ActivationStatus.Active, "PO123", PurchaseOrderStage.Created, Guid.NewGuid());
            var order = new ShippingOrder(Guid.NewGuid(), 1, CreateTestUser(), notApproved);

            var result = order.ShipOrder();

            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Message, Is.EqualTo("Order should be on approved stage to Start Shipping it."));
        }

        [Test]
        public void MarkOrderAsShipped_ShouldSucceed_WhenBeingShipped()
        {
            var order = new ShippingOrder(Guid.NewGuid(), 1, CreateTestUser(), CreateBeingShippedOrder());

            var result = order.MarkOrderAsShipped();

            Assert.IsTrue(result.IsSuccess);
            Assert.That(order.PackageOrder.OrderStage, Is.EqualTo(PurchaseOrderStage.Shipped));
            Assert.IsTrue(order.GetUncommittedEvents().Any());
        }

        [Test]
        public void MarkOrderAsShipped_ShouldFail_WhenNotBeingShipped()
        {
            var order = new ShippingOrder(Guid.NewGuid(), 1, CreateTestUser(), CreateApprovedPackageOrder());

            var result = order.MarkOrderAsShipped();

            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Message, Is.EqualTo("Order should be on BeingShipped stage to finish  shipment ."));
        }

        [Test]
        public void MarkOrderAsDelivered_ShouldSucceed_WhenShipped()
        {
            var shipped = new PackageOrder(Money.CreateInstance(100).Value, ActivationStatus.Active, "PO123", PurchaseOrderStage.Shipped, Guid.NewGuid());
            var order = new ShippingOrder(Guid.NewGuid(), 1, CreateTestUser(), shipped);

            var result = order.MarkOrderAsDelivered(new OrderClosed(Guid.NewGuid(), "PO123"));//TODO : Update it to fit

            Assert.IsTrue(result.IsSuccess);
            Assert.That(order.PackageOrder.OrderStage, Is.EqualTo(PurchaseOrderStage.Closed));
        }

        [Test]
        public void MarkOrderAsDelivered_ShouldFail_WhenNotShipped()
        {
            var order = new ShippingOrder(Guid.NewGuid(), 1, CreateTestUser(), CreateApprovedPackageOrder());

            var result = order.MarkOrderAsDelivered(new OrderClosed(Guid.NewGuid(), "PO123"));//TODO : Update it to fit

            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Message, Is.EqualTo("Order should be on Shipped stage to close  shipment ."));
        }
    }
}
