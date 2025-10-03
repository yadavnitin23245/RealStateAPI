# Real Estate API

A .NET 8 Web API for Real Estate management with JWT authentication, Entity Framework Core, and SQL Server.

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server (or Azure SQL Database)
- Docker (optional, for containerized deployment)

### Local Development

1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd API
   ```

2. **Update Connection String**
   Edit `RealState/appsettings.json` and update the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Your_SQL_Server_Connection_String"
   }
   ```

3. **Run the application**
   ```bash
   cd RealState
   dotnet run
   ```

4. **Access the API**
   - API: http://localhost:5276
   - Swagger UI: http://localhost:5276/swagger

## 🐳 Docker Deployment

### Build and Run with Docker

1. **Build the Docker image**
   ```bash
   docker build -t realstate-api .
   ```

2. **Run the container**
   ```bash
   docker run -p 8080:8080 -e "ConnectionStrings__DefaultConnection=Your_Connection_String" realstate-api
   ```

### Environment Variables

Set these environment variables for production:

- `ConnectionStrings__DefaultConnection`: SQL Server connection string
- `JwtSettings__Key`: JWT secret key
- `JwtSettings__Issuer`: JWT issuer
- `JwtSettings__Audience`: JWT audience
- `ASPNETCORE_ENVIRONMENT`: Set to "Production"

## 🚂 Railway Deployment

### Automatic Deployment

1. **Connect to Railway**
   - Push your code to GitHub
   - Connect your repository to Railway
   - Railway will automatically detect the Dockerfile and deploy

2. **Set Environment Variables**
   In Railway dashboard, add these environment variables:
   ```
   ConnectionStrings__DefaultConnection=Your_Production_Connection_String
   JwtSettings__Key=Your_JWT_Secret_Key
   JwtSettings__Issuer=RealStateAPI
   JwtSettings__Audience=RealStateAppUsers
   ASPNETCORE_ENVIRONMENT=Production
   ```

3. **Deploy**
   - Railway will automatically build and deploy your application
   - The health check endpoint will be available at `/health`

### Manual Deployment

1. **Install Railway CLI**
   ```bash
   npm install -g @railway/cli
   ```

2. **Login and Deploy**
   ```bash
   railway login
   railway init
   railway up
   ```

## 📁 Project Structure

```
API/
├── RealState/                 # Main API project
│   ├── Controllers/          # API controllers
│   ├── Services/             # Business services
│   ├── Email/                # Email utilities
│   └── Program.cs            # Application entry point
├── RealState.BAL/            # Business Logic Layer
│   ├── DTO/                  # Data Transfer Objects
│   ├── Logic/                # Business logic
│   └── ILogic/               # Interfaces
├── RealState.Data/           # Data Access Layer
│   ├── Models/               # Entity models
│   └── ApplicationDbContext.cs
├── RealState.Repository/     # Repository pattern
├── RealState.Common/         # Shared components
├── Dockerfile                # Docker configuration
├── .dockerignore             # Docker ignore file
└── railway.toml              # Railway configuration
```

## 🔧 API Endpoints

### Authentication
- `POST /Auth/login` - User login
- `GET /Auth/getUser` - Get current user (requires authentication)

### Contact
- Contact management endpoints (see Swagger UI)

### Health Check
- `GET /health` - Application health status

## 🔒 Security

- JWT Bearer token authentication
- HTTPS redirection in production
- Non-root user in Docker container
- Environment variable configuration

## 📊 Health Monitoring

The application includes a health check endpoint that:
- Verifies database connectivity
- Returns application status
- Provides version and environment information

## 🛠️ Development

### Adding New Features

1. Create DTOs in `RealState.BAL/DTO/`
2. Add business logic in `RealState.BAL/Logic/`
3. Create controllers in `RealState/Controllers/`
4. Update database models in `RealState.Data/Models/`

### Database Migrations

```bash
cd RealState
dotnet ef migrations add MigrationName
dotnet ef database update
```

## 📝 Notes

- The application uses Entity Framework Core with SQL Server
- JWT tokens are used for authentication
- Swagger UI is available in development mode
- Health checks are configured for container orchestration
- The Dockerfile uses multi-stage builds for optimization

## 🆘 Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Verify connection string
   - Ensure SQL Server is accessible
   - Check firewall settings

2. **JWT Token Issues**
   - Verify JWT settings in configuration
   - Ensure proper token format in requests

3. **Docker Build Fails**
   - Check .dockerignore file
   - Verify all project references
   - Ensure .NET 8.0 SDK is available

### Logs

Check application logs for detailed error information:
```bash
docker logs <container-id>
```

## 📄 License

This project is licensed under the MIT License. 