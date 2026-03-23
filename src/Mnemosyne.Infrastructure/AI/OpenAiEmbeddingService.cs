using System.Net;
using Microsoft.Extensions.Logging;
using Mnemosyne.Domain.Interfaces;
using OpenAI;
using OpenAI.Embeddings;
using Pgvector;

namespace Mnemosyne.Infrastructure.AI;

public class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly EmbeddingClient _embeddingClient;
    private readonly ILogger<OpenAiEmbeddingService> _logger;
    private const int MaxRetries = 3;
    private const int InitialRetryDelayMs = 1000;

    public OpenAiEmbeddingService(EmbeddingClient embeddingClient, ILogger<OpenAiEmbeddingService> logger)
    {
        _embeddingClient = embeddingClient;
        _logger = logger;
    }

    public async Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be empty", nameof(text));
        }

        var attempt = 0;
        Exception? lastException = null;

        while (attempt < MaxRetries)
        {
            try
            {
                var result = await _embeddingClient.GenerateEmbeddingAsync(text, null, cancellationToken);
                var embedding = result.Value;
                
                if (embedding == null)
                {
                    return null;
                }

                var vector = embedding.ToFloats();
                return new Vector(vector.ToArray());
            }
            catch (Exception ex) when (IsTransientError(ex) && attempt < MaxRetries - 1)
            {
                lastException = ex;
                attempt++;
                var delay = InitialRetryDelayMs * Math.Pow(2, attempt - 1);
                _logger.LogWarning("OpenAI API transient error on attempt {Attempt}/{MaxRetries}. Retrying in {Delay}ms. Error: {Error}",
                    attempt, MaxRetries, delay, ex.Message);
                await Task.Delay((int)delay, cancellationToken);
            }
        }

        if (lastException != null)
        {
            _logger.LogError(lastException, "Failed to generate embedding after {MaxRetries} attempts", MaxRetries);
            throw lastException;
        }

        return null;
    }

    private static bool IsTransientError(Exception ex)
    {
        // Check for rate limiting (429) and server errors (5xx)
        if (ex.Message.Contains("429") || 
            ex.Message.Contains("insufficient_quota") ||
            ex.Message.Contains("rate_limit") ||
            ex.Message.Contains("500") ||
            ex.Message.Contains("502") ||
            ex.Message.Contains("503") ||
            ex.Message.Contains("504"))
        {
            return true;
        }

        // Check for network-related exceptions
        if (ex is HttpRequestException || 
            ex is TaskCanceledException ||
            ex is TimeoutException)
        {
            return true;
        }

        return false;
    }
}
