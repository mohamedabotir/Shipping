using Application.Handlers;
using Infrastructure.Consumer.Usecases;
using Common.Repository;
using Confluent.Kafka;
using Domain.Repositories;
using Infrastructure.Consumer.Context;
using Infrastructure.Consumer.Repository;
using Infrastructure.Consumer;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using EventHandler = Application.Handlers.EventHandler;

var builder = WebApplication.CreateBuilder(args);
Action<DbContextOptionsBuilder> dbContextConfiguration = (e => e.UseSqlServer(builder.Configuration.GetConnectionString("ShippingOrder")));
builder.Services.AddDbContext<ShippingOrderContext>(dbContextConfiguration);
builder.Services.AddSingleton(new ShoppingContextFactory(dbContextConfiguration));

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("ConsumerConfig"));
builder.Services.Configure<Topic>(builder.Configuration.GetSection("Topic"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IPlaceShipmentRequestUsecase, PlaceShipmentRequestUsecase>();
builder.Services.AddTransient<IEventHandler, EventHandler>();
builder.Services.AddTransient<IEventConsumer<EventConsumer>, EventConsumer>();
builder.Services.AddTransient<IShippingRepository, ShippingRepository>();
builder.Services.AddHostedService<ConsumerHostingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
