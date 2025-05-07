# Shipping Service
![Coverage](https://img.shields.io/badge/coverage-66%25-brightgreen)
This document outlines the design and implementation details of the **Shipping Service** using **Domain-Driven Design (DDD)** , **Kafka**, **MongoDB**, **SQL Server**, and **Docker**.

---

## Architecture Overview

### Key Components

1. **Domain-Driven Design (DDD)**:
   - Focus on the core domain and its logic.
   - Use aggregates, entities, value objects, repositories, and services.

2. **Kafka**:
   - Acts as a message broker for event-driven communication.
   - Handles events published and consumed by the service.

3. **MongoDB**:
   - Used as the event store to persist raised events.

4. **SQL Server**:
   - Stores transactional data related to shipping.

5. **Docker**:
   - Runs instances of Kafka, MongoDB, and SQL Server for containerized deployments.

---

## Service Features

### 1. **Place Shipment Request**
   - Initiates a shipment for a placed order.
   - Publishes a `OrderBeingShipped` event to Kafka.

### 2. **Start Shipping**
   - Handles `PurchaseOrderApproved` events from the Purchase Order Service.
   - Validates the state of the purchase order before initiating shipping.
     
### 3. **Ship Order**
   - Handles the process of shipping an order.
   - Updates the domain state and publishes a `OrderShipped` event to Kafka.

---

## Domain Model

### Aggregate Root
- **ShippingOrder**:
  - Attributes: `Guid`, `OrderId`, `User`, `PackageOrder`
  - Methods: `StartShipping()`, `ShipOrder()`

### Value Objects
- **PackageOrder**:
  - Attributes: `TotalAmount (Money)`, `ActivationStatus`, `PurchaseOrderNumber`, `PurchaseOrderStage`, `PoGuid`

- **User**:
  - Attributes: `Name`, `Address`, `PhoneNumber`

### Repositories
- `IEventRepository`: Manages event persistence.
- `IShippingRepository`: Manages ShippingOrder aggregates.

---

## Kafka Event Design

### Event Types

#### Published Events
1. **OrderBeingShipped**:
   - Raised when the shipping process is initiated.

2. **OrderShipped**:
   - Raised when an order is shipped.

#### Consumed Events
1. **PurchaseOrderApproved**:
   - Triggered by the Purchase Order Service when an order is approved.

2. **OrderClosed**:
   - Triggered by the Inventory Service as a reflection of the `OrderShipped` event.

### Kafka Configuration
- Topic: `shippingOrder`
- Partitions: Based on `ShippingOrder.Guid`

### MongoDB Event Store
- Database: `ERPEventStore`
- Collection: `ShippingOrder`
- Schema:
  ```json
  {
    "_id": "<EventId>",
    "shippingOrderId": "<ShippingOrderId>",
    "eventType": "<EventType>",
    "data": { <EventData> },
    "timestamp": "<Timestamp>"
  }
  ```

---

## Use Cases

### **OrderShippedUseCase**
Handles the logic to mark an order as shipped and publish related events.

### **PlaceShipmentRequestUseCase**
Handles the initiation of a shipment request for an order.

### **ShipOrderUseCase**
Manages the process of shipping an order, including validations and event publishing.

---

## Handlers

### **StartShippingHandler**
Handles `PurchaseOrderApproved` and `OrderClosed` events.

- Method:
  ```csharp
  public Task On(PurchaseOrderApproved @event);
  public Task On(OrderClosed @event);
  ```

### **DocumentAsShippedHandler**
Handles the logic to mark a shipping order as shipped and trigger related events.

---

## Event Consumption

### Hosted Service
- **ConsumerHostingService**:
  - Consumes events from the `shippingOrder` topic.
  - Processes `PurchaseOrderApproved` and `OrderClosed` events.

### GraphQL Endpoint
- Validates the state of the purchase order to ensure it is not deactivated before raising an `OrderBeingShipped` event.

---

## Data Mapping

### POCOs and Domain Objects
- Use separate POCOs for data transfer and persistence.
- Map POCOs to domain objects using mapping utilities.

---

## Future Enhancements
1. Add retries for failed Kafka event processing.
2. Add unit testing

---
