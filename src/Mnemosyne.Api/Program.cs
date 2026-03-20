using Microsoft.EntityFrameworkCore;
using Mnemosyne.Application.Features.Auth.ValidateApiKey;
using Mnemosyne.Application.Features.Compress.CompressContext;
using Mnemosyne.Application.Features.Index.GetIndexStatus;
using Mnemosyne.Application.Features.Index.StartProjectIndex;
using Mnemosyne.Application.Features.Memory.CreateMemory;
using Mnemosyne.Application.Features.Memory.SearchMemory;
using Mnemosyne.Application.Features.Project.CreateProject;
using Mnemosyne.Application.Features.Project.DeleteProject;
using Mnemosyne.Application.Features.Project.GetProject;
using Mnemosyne.Application.Features.Project.ListProjects;
using Mnemosyne.Application.Features.Project.UpdateProject;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Domain.Services;
using Mnemosyne.Infrastructure.Compression;
using Mnemosyne.Infrastructure.Persistence;
using Mnemosyne.Infrastructure.Repositories;
using Mnemosyne.Api.Endpoints;
using Mnemosyne.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MnemosyneDb")
    ?? throw new InvalidOperationException("Connection string 'MnemosyneDb' not found.");

builder.Services.AddDbContext<MnemosyneDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IMemoryRepository, MemoryRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectIndexJobRepository, ProjectIndexJobRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<CreateMemoryHandler>();
builder.Services.AddScoped<SearchMemoryHandler>();
builder.Services.AddScoped<ValidateApiKeyHandler>();
builder.Services.AddScoped<CreateProjectHandler>();
builder.Services.AddScoped<GetProjectHandler>();
builder.Services.AddScoped<ListProjectsHandler>();
builder.Services.AddScoped<UpdateProjectHandler>();
builder.Services.AddScoped<DeleteProjectHandler>();
builder.Services.AddScoped<StartProjectIndexHandler>();
builder.Services.AddScoped<GetIndexStatusHandler>();

builder.Services.AddSingleton<ICompressionStrategy, CodeStructureCompressionStrategy>();
builder.Services.AddScoped<CompressContextHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapAuthEndpoints();
app.UseApiKeyValidation();

app.MapMemoryEndpoints();
app.MapProjectEndpoints();
app.MapCompressEndpoints();

app.MapHealthEndpoints();

app.Run();

public partial class Program { }
