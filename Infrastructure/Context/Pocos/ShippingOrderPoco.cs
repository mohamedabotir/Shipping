using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Constants;
using Common.Result;
using Common.ValueObject;
using Domain.Entity;
using Domain.ValueObject;

namespace Infrastructure.Consumer.Context.Pocos;
[Table("ShippingOrder")]
public class ShippingOrderPoco
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }
    [Required]
    public Guid Guid { get; set; }
    [Required]
    [StringLength(50)] 
    public string CustomerName { get; set; }
    [Required]
    [MinLength(10)]
    [MaxLength(256)]
    public string CustomerAddress { get; set; }
    [Required]
    [StringLength(15)] 
    public string CustomerPhoneNumber { get; set; }
    [Required]
    public decimal TotalAmount { get; set; }
    [Required]
    [Column(name:"IsActive")]
    public ActivationStatus ActivationStatus { get; private set; }
    [Required]
    [MaxLength(25)]
    public string PurchaseOrderNumber { get; protected set; }
    [Required]
    public PurchaseOrderStage OrderStage { get; set; }

    [Required]
    public Guid PurchaseOrderGuid { get; set; }
    public Result<ShippingOrder> MapPocoToDomain()
    {
        var address = Address.CreateInstance(CustomerAddress);
        if (address.IsFailure)
            return Result.Fail<ShippingOrder>(address.Message);
        var totalAmount = Money.CreateInstance(TotalAmount);
        var user = User.CreateInstance(CustomerName, address.Value,CustomerPhoneNumber);
        var combinationResult = Result.Combine(totalAmount, user);
        
        if (combinationResult.IsFailure)
            return Result.Fail<ShippingOrder>(combinationResult.Message);
        var package = new PackageOrder(totalAmount.Value,ActivationStatus,PurchaseOrderNumber,OrderStage,PurchaseOrderGuid);
        var shippingOrder = new ShippingOrder(Guid,OrderId, user.Value,package);
        return Result.Ok(shippingOrder);
    }

    public ShippingOrderPoco MapDomainToPoco(ShippingOrder shippingOrder)
    {
        Guid = shippingOrder.Guid;
        CustomerName = shippingOrder.Customer.Name;
        CustomerAddress = shippingOrder.Customer.Address.AddressValue;
        CustomerPhoneNumber = shippingOrder.Customer.PhoneNumber;
        TotalAmount = shippingOrder.PackageOrder.TotalAmount.MoneyValue;
        ActivationStatus = shippingOrder.PackageOrder.ActivationStatus;
        PurchaseOrderNumber = shippingOrder.PackageOrder.PurchaseOrderNumber;
        OrderStage = shippingOrder.PackageOrder.OrderStage;
        PurchaseOrderGuid = shippingOrder.PackageOrder.PurchaseOrderGuid;
        return this;
    }
}