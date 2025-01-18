CREATE TABLE [ShippingOrder] (
    OrderId INT IDENTITY(1,1) PRIMARY KEY, 
    Guid UNIQUEIDENTIFIER NOT NULL,       
    CustomerName NVARCHAR(50) NOT NULL, 
    CustomerAddress NVARCHAR(256) NOT NULL, 
    CustomerPhoneNumber NVARCHAR(15) NOT NULL, 
    TotalAmount DECIMAL(18,2) NOT NULL,  
    IsActive INT NOT NULL,              
    PurchaseOrderNumber NVARCHAR(25) NOT NULL, 
    PurchaseOrderGuid UNIQUEIDENTIFIER NOT NULL, 
    OrderStage INT NOT NULL             
);

