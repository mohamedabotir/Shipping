using Common.Events;
using Common.Mongo.Producers;
using Common.Repository;
using Common.ValueObject;
using Confluent.Kafka;
using Domain.ValueObject;
using Infrastructure.Configs;
using Infrastructure.Consumer;
using Infrastructure.MessageBroker.Producers;
using Infrastructure.Mongo;
using Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Serilog;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

SerilogConfigurator.Configure(config);

try
{
    Log.Information("Starting background job...");

    var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.AddConfiguration(config);
        }).UseSerilog()
        .ConfigureServices((context, services) =>
        {
            services.Configure<Topic>(context.Configuration.GetSection("Topic"));
            services.Configure<ShippingOrderConfig>(context.Configuration.GetSection("MongoConfig"));
            services.Configure<ProducerConfig>(context.Configuration.GetSection("ProducerConfig"));
            services.Configure<ElkLog>(context.Configuration.GetSection("ElkLog"));

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BsonClassMap.RegisterClassMap<DomainEventBase>();
            BsonClassMap.RegisterClassMap<OrderBeingShipped>();
            BsonClassMap.RegisterClassMap<OrderShipped>();
            BsonClassMap.RegisterClassMap<PurchaseOrderApproved>();
            BsonClassMap.RegisterClassMap<EventModel>();
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));

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
