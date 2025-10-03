# Use .NET 8 SDK to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Set correct working directory for solution
WORKDIR /src/RealState

# Restore dependencies
RUN dotnet restore RealState.sln

# Publish main project — adjust if csproj is deeper
RUN dotnet publish RealState.csproj -c Release -o /app/publish

# Use runtime image for .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RealState.dll"]
