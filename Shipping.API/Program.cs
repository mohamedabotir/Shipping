using System.Reflection;
using Application.Commands;
using Application.Extensions;
using Application.Handlers;
using Application.Usecases;
using Common.Constants;
using Common.Events;
using Common.Handlers;
using Common.Mongo.Producers;
using Infrastructure.Consumer.Usecases;
using Common.Repository;
using Common.Result;
using Confluent.Kafka;
using Domain.Repositories;
using Infrastructure.Consumer.Context;
using Infrastructure.Consumer.Repository;
using Infrastructure.Consumer;
using Infrastructure.Context;
using Infrastructure.GraphQL;
using Infrastructure.MessageBroker;
using Infrastructure.MessageBroker.Producers;
using Infrastructure.Mongo;
using Infrastructure.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using EventHandler = Application.Handlers.EventHandler;
using Domain.Entity;
using Infrastructure.Configs;

var builder = WebApplication.CreateBuilder(args);
Action<DbContextOptionsBuilder> dbContextConfiguration = (e => e.UseSqlServer(builder.Configuration.GetConnectionString("ShippingOrder")));
builder.Services.AddDbContext<ShippingOrderContext>(dbContextConfiguration);
builder.Services.AddSingleton(new ShippingContextFactory(dbContextConfiguration));

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("ConsumerConfig"));
builder.Services.Configure<Topic>(builder.Configuration.GetSection("Topic"));
builder.Services.Configure<TopicShippingOrders>(builder.Configuration.GetSection("TopicShippingOrders"));
builder.Services.Configure<ShippingOrderConfig>(builder.Configuration.GetSection("MongoConfig"));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection("ProducerConfig"));
builder.Services.Configure<ElkLog>(builder.Configuration.GetSection("ElkLog"));

builder.Services.Configure<PurchaseOrderGraphQLEndpoint>(builder.Configuration.GetSection(nameof(PurchaseOrderGraphQLEndpoint)));
SerilogConfigurator.Configure(builder.Configuration);


BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
BsonClassMap.RegisterClassMap<DomainEventBase>();
BsonClassMap.RegisterClassMap<OrderBeingShipped>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IUnitOfWork<ShippingOrder>, UnitOfWork>();
builder.Services.AddTransient<IEventConsumer<EventConsumer>, EventConsumer>();
builder.Services.AddTransient<IEventRepository, EventRepository>();
builder.Services.AddTransient<IShippingRepository, ShippingRepository>();
builder.Services.AddTransient<IEventSourcing<ShippingOrder>, EventSourcing>();
builder.Services.AddTransient<IProducer,Producer>();
// UseCases
builder.Services.AddTransient<IPlaceShipmentRequestUsecase, PlaceShipmentRequestUsecase>();
builder.Services.AddTransient<IShipOrderUsecase, ShipOrderUseCase>();
builder.Services.AddTransient<IOrderShippedUseCase, OrderShippedUseCase>();
builder.Services.AddTransient<IClosingShipmentRequestUseCase, ClosingShipmentRequestUseCase>();

// Handlers
builder.Services.AddTransient<IEventHandler, EventHandler>();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddTransient<IRequestHandler<StartShippingCommand,Result>, StartShippingHandler>();
builder.Services.AddTransient<IRequestHandler<OrderShippedCommand,Result>, DocumentAsShippedHandler>();

builder.Services.AddTransient<IProducer, Producer>();
builder.Services.AddTransient<IEventDispatcher, EventDispatcher>();
builder.Services.AddTransient<GraphQlClient>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHostedService<ConsumerHostingService>();
var app = builder.Build();
 
    app.UseSwagger();
    app.UseSwaggerUI();
 
app.UseHttpsRedirection();
app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.MapGet("/order/{poNumber}", async (string poNumber,GraphQlClient ql) =>
    {
        var result = await ql.GetActivationStatus(poNumber);
        return Results.Ok(result);
    })
    .WithName("approve  order")
    .WithOpenApi();
app.MapPost("/shipOrder/{poNumber}/startShip", async (string poNumber,IMediator mediator,GraphQlClient graphQlClient) =>
    {
        
       var isPurchaseOrderActive = await graphQlClient.GetActivationStatus(poNumber) == ActivationStatus.Active;
       if (!isPurchaseOrderActive)
       {
           return Results.BadRequest(Result.Fail("Order is not active"));
       }
       var result =await mediator.Send(new StartShippingCommand(poNumber));
       return Results.Ok(result);
    })
    .WithName("start ship order")
    .WithOpenApi();
app.MapPost("/shipOrder/{poNumber}/shipped", async (string poNumber,IMediator mediator) =>
    {
        
        var result =await mediator.Send(new OrderShippedCommand(poNumber));
        return Results.Ok(result);
    })
    .WithName("order shipped")
    .WithOpenApi();

app.Run();
