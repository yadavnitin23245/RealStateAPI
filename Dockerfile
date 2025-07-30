# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy solution file to the working directory
COPY RealState/*.sln ./

# Copy project files
COPY RealState/*.csproj ./RealState/
COPY RealState.BAL/*.csproj ./RealState.BAL/
COPY RealState.Common/*.csproj ./RealState.Common/
COPY RealState.Data/*.csproj ./RealState.Data/
COPY RealState.Repository/*.csproj ./RealState.Repository/

# Restore dependencies
RUN dotnet restore RealState.sln

# Copy the entire source code
COPY . .

# Build and publish the application
RUN dotnet publish RealState/RealState.csproj -c Release -o /app/publish

# Stage 2: Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose port (optional: match your app's launchSettings.json if needed)
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "RealState.dll"]
