using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks.BankSimulator;
using PaymentGateway.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<IAcquiringBankApiClient, BankSimulatorApiClient>((serviceProvider, client) =>
{
    client.BaseAddress = new Uri("http://localhost:8080");
});

builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
builder.Services.AddTransient<IPaymentService, PaymentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
