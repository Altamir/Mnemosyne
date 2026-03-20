using Mnemosyne.Application.Features.Auth.ValidateApiKey;

namespace Mnemosyne.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

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

public record ValidateApiKeyRequest(string ApiKey);