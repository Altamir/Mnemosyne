using Mnemosyne.Application.Features.Auth.CreateApiKey;
using Mnemosyne.Application.Features.Auth.ValidateApiKey;

namespace Mnemosyne.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        // Public endpoint to create a new API key (no authentication required)
        group.MapPost("/keys", async (CreateApiKeyRequest request, CreateApiKeyHandler handler, CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new CreateApiKeyCommand(request.Email);
                var result = await handler.Handle(command, cancellationToken);
                return Results.Ok(new { userId = result.UserId, apiKey = result.ApiKey, email = result.Email, createdAt = result.CreatedAt });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CreateApiKey")
        .WithSummary("Creates a new API key")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/validate", async (ValidateApiKeyRequest request, ValidateApiKeyHandler handler, CancellationToken cancellationToken) =>
        {
            try
            {
                var query = new ValidateApiKeyQuery(request.ApiKey);
                var user = await handler.Handle(query, cancellationToken);
                
                if (user is null)
                {
                    return Results.Unauthorized();
                }
                
                return Results.Ok(new { userId = user.Id, email = user.Email });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("ValidateApiKey")
        .WithSummary("Validates an API Key")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}

public record CreateApiKeyRequest(string Email);
public record ValidateApiKeyRequest(string ApiKey);