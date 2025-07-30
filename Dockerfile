# Use the official .NET 8.0 SDK image as the build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the solution file first for better layer caching
COPY *.sln ./

# Copy all project files
COPY RealState/RealState.csproj RealState/
COPY RealState.BAL/RealState.BAL.csproj RealState.BAL/
COPY RealState.Data/RealState.Data.csproj RealState.Data/
COPY RealState.Repository/RealState.Repository.csproj RealState.Repository/
COPY RealState.Common/RealState.Common.csproj RealState.Common/

# Restore dependencies for all projects
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build --no-restore --configuration Release

# Publish the application
RUN dotnet publish RealState/RealState.csproj --no-restore --configuration Release --output /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory
WORKDIR /app

# Install necessary packages for production
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose port 8080 (Railway uses this port by default)
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "RealState.dll"] 