using Serilog;
using Kafka.Consumer.Service.Service;
using Kafka.Consumer.Service.Infrastructure.Settings;
using Kafka.Consumer.Service.Infrastructure.Repositories;
using Kafka.Consumer.Service.Infrastructure.Interfaces;
using Kafka.Consumer.Service.Infrastructure.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddHostedService<ConsumerHostedService>();
builder.Services.Configure<TopicSettings>(builder.Configuration.GetSection(nameof(TopicSettings)));
builder.Services.AddSingleton<IConsumerConnection, ConsumerConnection>();
builder.Services.AddTransient<IEventListener, TopicRepository>();

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.Run();
