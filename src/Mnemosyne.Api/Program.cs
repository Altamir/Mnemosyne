using Microsoft.EntityFrameworkCore;
using Mnemosyne.Application.Features.Memory.CreateMemory;
using Mnemosyne.Application.Features.Memory.SearchMemory;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;
using Mnemosyne.Infrastructure.Repositories;
using Mnemosyne.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MnemosyneDb")
    ?? throw new InvalidOperationException("Connection string 'MnemosyneDb' not found.");

builder.Services.AddDbContext<MnemosyneDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IMemoryRepository, MemoryRepository>();
builder.Services.AddScoped<CreateMemoryHandler>();
builder.Services.AddScoped<SearchMemoryHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapMemoryEndpoints();

app.MapHealthEndpoints();

app.Run();
