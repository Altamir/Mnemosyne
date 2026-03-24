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
using Mnemosyne.Infrastructure.AI;
using Mnemosyne.Infrastructure.Compression;
using Mnemosyne.Infrastructure.Persistence;
using Mnemosyne.Infrastructure.Repositories;
using Mnemosyne.Infrastructure.Services;
using Mnemosyne.Api.Configuration;
using Mnemosyne.Api.Endpoints;
using Mnemosyne.Api.GrpcServices;
using Mnemosyne.Api.Middleware;
using OpenAI;
using OpenAI.Embeddings;
using Scalar.AspNetCore;

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

// OpenAI Embedding Service configuration
var openAiApiKey = builder.Configuration["OpenAI:ApiKey"] 
    ?? throw new InvalidOperationException("OpenAI:ApiKey configuration is required.");
var embeddingModel = builder.Configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";

builder.Services.AddSingleton(new OpenAIClient(openAiApiKey));
builder.Services.AddSingleton<EmbeddingClient>(sp => 
{
    var client = sp.GetRequiredService<OpenAIClient>();
    return client.GetEmbeddingClient(embeddingModel);
});
builder.Services.AddSingleton<IEmbeddingService, OpenAiEmbeddingService>();

builder.Services.AddHostedService<ProjectIndexerService>();

// gRPC services
builder.Services.AddGrpc();

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck<PostgreSqlHealthCheck>("postgresql")
    .AddCheck<OpenAiHealthCheck>("openai");

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new Microsoft.OpenApi.OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["ApiKey"] = new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.SecuritySchemeType.ApiKey,
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Name = "X-Api-Key",
            Description = "Chave de API para autenticação. Obtenha sua chave em POST /api/v1/auth/keys."
        };

        // Apply security requirements per operation instead of globally,
        // so that public routes like /api/v1/auth/* and /health* remain unauthenticated.
        document.Security ??= new List<Microsoft.OpenApi.OpenApiSecurityRequirement>();
        var globalRequirement = new Microsoft.OpenApi.OpenApiSecurityRequirement();
        globalRequirement[new Microsoft.OpenApi.OpenApiSecuritySchemeReference("ApiKey", document, null)] = new List<string>();
        document.Security.Add(globalRequirement);

        if (document.Paths != null)
        {
            var schemeRef = new Microsoft.OpenApi.OpenApiSecuritySchemeReference("ApiKey", document, null);
            var requirement = new Microsoft.OpenApi.OpenApiSecurityRequirement();
            requirement[schemeRef] = new List<string>();

            foreach (var path in document.Paths)
            {
                var pathTemplate = path.Key ?? string.Empty;
                var isPublic =
                    pathTemplate.StartsWith("/api/v1/auth", StringComparison.OrdinalIgnoreCase) ||
                    pathTemplate.StartsWith("/health", StringComparison.OrdinalIgnoreCase);

                var pathItem = path.Value;
                if (pathItem?.Operations == null)
                {
                    continue;
                }

                foreach (var operation in pathItem.Operations.Values)
                {
                    if (isPublic)
                    {
                        // Ensure public endpoints are documented without API key requirements
                        operation.Security = new List<Microsoft.OpenApi.OpenApiSecurityRequirement>();
                    }
                    else
                    {
                        operation.Security ??= new List<Microsoft.OpenApi.OpenApiSecurityRequirement>();
                        operation.Security.Add(requirement);
                    }
                }
            }
        }
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.MapAuthEndpoints();
app.UseApiKeyValidation();

app.MapMemoryEndpoints();
app.MapProjectEndpoints();
app.MapCompressEndpoints();

// gRPC endpoints
app.MapGrpcService<SearchGrpcService>();
app.MapGrpcService<IndexGrpcService>();
app.MapGrpcService<CompressGrpcService>();

app.MapHealthEndpoints();

app.Run();

public partial class Program { }
