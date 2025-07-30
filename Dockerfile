# Use .NET SDK for build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy everything to container
COPY . .

# Go into the RealState folder where the .sln exists
WORKDIR /src/RealState

# Restore using the solution file
RUN dotnet restore RealState.sln

# Build and publish main web/app project
# Adjust path if main project is inside /RealState/RealState/
RUN dotnet publish RealState.csproj -c Release -o /app/publish

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RealState.dll"]
