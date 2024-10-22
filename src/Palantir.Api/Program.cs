using Microsoft.Extensions.Configuration;
using Palantir.Api.Configurations;
using Palantir.Api.Interfaces;
using Palantir.Api.Services;
using static Palantir.Api.Models.ClickUpTaskModel;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<ClickUpSettings>(builder.Configuration.GetSection("ClickUpSettings"));
builder.Services.Configure<HubSpotSettings>(builder.Configuration.GetSection("HubSpotSettings"));

builder.Services.AddSingleton<IDevelopmentTaskService<HubSpotTicketProperties, TaskList>, ClickUpService>();
builder.Services.AddSingleton<ICustomerTicketService<HubSpotTicketProperties>, HubSpotService>();

builder.Services.AddHttpClient<ClickUpService>();
builder.Services.AddHttpClient<HubSpotService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
