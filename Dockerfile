# Use official .NET 8 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o out

# Use runtime-only image for final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/out ./

# Expose Prometheus metrics port
EXPOSE 9091

# Set environment variable for production
ENV DOTNET_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "SilentLogAgent.dll"]