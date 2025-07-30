# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy solution and project files
COPY RealState/*.sln ./RealState/
COPY RealState/*.csproj ./RealState/
COPY RealState.BAL/*.csproj ./RealState.BAL/
COPY RealState.Common/*.csproj ./RealState.Common/
COPY RealState.Data/*.csproj ./RealState.Data/
COPY RealState.Repository/*.csproj ./RealState.Repository/

# Restore dependencies
RUN dotnet restore RealState/RealState.sln

# Copy the rest of the source code
COPY . .

# Build and publish
RUN dotnet publish RealState/RealState.sln -c Release -o /app/publish

# Stage 2 - Run
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RealState.dll"]
