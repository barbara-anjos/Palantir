using Microsoft.Extensions.Configuration;
using Palantir.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Carregar as configurações do ClickUp
builder.Services.Configure<ClickUpSettings>(builder.Configuration.GetSection("ClickUpSettings"));

// Carregar as configurações do HubSpot
builder.Services.Configure<HubSpotSettings>(builder.Configuration.GetSection("HubSpotSettings"));

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
