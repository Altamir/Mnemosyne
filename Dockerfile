# Build stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY Mnemosyne.slnx ./
COPY src/Mnemosyne.Api/Mnemosyne.Api.csproj src/Mnemosyne.Api/
COPY src/Mnemosyne.Application/Mnemosyne.Application.csproj src/Mnemosyne.Application/
COPY src/Mnemosyne.Domain/Mnemosyne.Domain.csproj src/Mnemosyne.Domain/
COPY src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj src/Mnemosyne.Infrastructure/

# Restore dependencies
RUN dotnet restore src/Mnemosyne.Api/Mnemosyne.Api.csproj

# Copy all source code
COPY . .

# Build and publish
WORKDIR /src/src/Mnemosyne.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user
RUN useradd -m -s /bin/sh appuser

# Copy published files
COPY --from=build /app/publish .

# Change ownership to non-root user
RUN chown -R appuser:appuser /app
USER appuser

EXPOSE 8080
EXPOSE 8081

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "Mnemosyne.Api.dll"]
