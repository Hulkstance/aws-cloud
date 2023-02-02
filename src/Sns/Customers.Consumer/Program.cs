using Amazon.SQS;
using Customers.Consumer.Models;
using Customers.Consumer.Services;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(QueueSettings.Key));
builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
builder.Services.AddHostedService<QueueConsumerService>();
builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

app.Run();
