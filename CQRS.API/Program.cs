using CQRS.Core.Services;
using CQRS.Infrastructure.MessageBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMessageBusService, MessageBusService>();

builder.Services.AddSingleton<MessageBusConsumer>();

var app = builder.Build();

var messageBus = app.Services.GetService<MessageBusConsumer>();

messageBus.RegisterHandler();

app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

app.Run();
