using System.ComponentModel;

namespace Common.Utils;

public enum  NumberGenerator
{
    [Description("Po-yymmdd")]
    PoAndyymmdd,
    [Description("Po{timestamp}")]
    PoTimestamp,
    [Description("SKU-3-3-3")]
    Sku,
    [Description("SHO-yymmdd-poNumber")]
    ShippingNumber
}