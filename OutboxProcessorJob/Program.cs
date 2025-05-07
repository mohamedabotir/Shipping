using Infrastructure.Producers;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Common.Events;
using Infrastructure.Mongo;
using Common.Repository;
using Domain.DomainEvents;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Confluent.Kafka;
using Infrastructure.Exceptions;
using Serilog;
using Common.ValueObject;
using Domain.Entity;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

SerilogConfigurator.Configure(config);

try
{
    Log.Information("Starting background job...");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.AddConfiguration(config);
        })
        .ConfigureServices((context, services) =>
        {
            services.Configure<Topic>(context.Configuration.GetSection("Topic"));
            services.Configure<PurchaseOrderConfig>(context.Configuration.GetSection("MongoConfig"));
            services.Configure<ProducerConfig>(context.Configuration.GetSection("ProducerConfig"));
            services.Configure<ElkLog>(context.Configuration.GetSection("ElkLog"));

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BsonClassMap.RegisterClassMap<DomainEventBase>();
            BsonClassMap.RegisterClassMap<PoCreatedEventBase>();
            BsonClassMap.RegisterClassMap<PurchaseOrderApproved>();
            BsonClassMap.RegisterClassMap<OrderBeingShipped>();
            BsonClassMap.RegisterClassMap<EventModel>();
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));

            BsonClassMap.RegisterClassMap<Email>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Address>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<Quantity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<Money>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<Item>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<LineItem>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            services.AddSingleton<IEventRepository, EventRepository>();
            services.AddSingleton<IProducer, Producer>();
            services.AddTransient<OutboxProcessor>();
        })
        .Build();

    using var scope = host.Services.CreateScope();
    var processor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();
    await processor.RunAsync(CancellationToken.None);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Job host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush(); 
}
