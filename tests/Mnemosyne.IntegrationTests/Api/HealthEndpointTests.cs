using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mnemosyne.Infrastructure.Persistence;
using System.Net;
using Xunit;

namespace Mnemosyne.IntegrationTests.Api;

public class HealthEndpointTests : IClassFixture<HealthWebApplicationFactory>, IDisposable
{
    private readonly HealthWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public HealthEndpointTests(HealthWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    [Fact(DisplayName = "GET /health/live - retorna 200 OK")]
    [Trait("Layer", "Integration - API - Health")]
    public async Task HealthLive_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("alive", body);
    }

    [Fact(DisplayName = "GET /health/ready - retorna 200 quando banco conectado")]
    [Trait("Layer", "Integration - API - Health")]
    public async Task HealthReady_WhenDbConnected_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("ready", body);
        Assert.Contains("connected", body);
    }
}

public class HealthWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<MnemosyneDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<MnemosyneDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });
    }
}
