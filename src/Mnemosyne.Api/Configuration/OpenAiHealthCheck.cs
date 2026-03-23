using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenAI;

namespace Mnemosyne.Api.Configuration;

public class OpenAiHealthCheck : IHealthCheck
{
    private readonly OpenAIClient _openAiClient;

    public OpenAiHealthCheck(OpenAIClient openAiClient)
    {
        _openAiClient = openAiClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to get a simple embedding to verify OpenAI API is accessible
            // Using a minimal text to reduce cost
            var embeddingClient = _openAiClient.GetEmbeddingClient("text-embedding-3-small");
            var result = await embeddingClient.GenerateEmbeddingAsync("test", null, cancellationToken);
            
            if (result.Value != null)
            {
                return HealthCheckResult.Healthy("OpenAI API is accessible");
            }
            
            return HealthCheckResult.Unhealthy("OpenAI API returned null response");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                $"OpenAI API health check failed: {ex.Message}",
                ex);
        }
    }
}
