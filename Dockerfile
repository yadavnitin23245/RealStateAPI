# Use the SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy only the necessary project files first
COPY RealState/*.sln ./RealState/
COPY RealState/RealState/*.csproj ./RealState/RealState/
COPY RealState.RealState.BAL/*.csproj ./RealState.RealState.BAL/
COPY RealState.Common/*.csproj ./RealState.Common/
COPY RealState.Data/*.csproj ./RealState.Data/
COPY RealState.Repository/*.csproj ./RealState.Repository/

# Restore dependencies
WORKDIR /src/RealState
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Publish the app
WORKDIR /src/RealState
RUN dotnet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RealState.dll"]
