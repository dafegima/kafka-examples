using Kafka.Producer.API.Infrastructure.Interfaces;
using Kafka.Producer.API.Infrastructure.Repositories;
using Kafka.Producer.API.Infrastructure.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc)=> lc.ReadFrom.Configuration(ctx.Configuration));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<TopicSettings>(builder.Configuration.GetSection(nameof(TopicSettings)));
builder.Services.AddTransient<ITopicRepository, TopicRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
