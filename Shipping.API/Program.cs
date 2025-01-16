using Application.Usecases;
using Confluent.Kafka;
using Domain.Entity;
using Domain.Repositories;
using Infrastructure.Consumer;
using Infrastructure.Repository;
using EventHandler = Infrastructure.Consumer.EventHandler;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("ConsumerConfig"));
builder.Services.Configure<Topic>(builder.Configuration.GetSection("Topic"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ConsumerHostingService>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<IPlaceShipmentRequestUsecase, PlaceShipmentRequestUsecase>();
builder.Services.AddScoped<IEventHandler, EventHandler>();
builder.Services.AddScoped<IEventConsumer<EventConsumer>, EventConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
