using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealState.BAL.DTO;
using RealState.BAL.ILogic;
using RealState.BAL.Logic;
using RealState.Data;
using RealState.Data.Models;
using RealState.Repository;
using RealState.Repository.Repository;
using RealState.Services;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ? JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

// ? JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ? Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? Register Repositories and Logic Layer
builder.Services.AddScoped<IRepository<Contact>, Repository<Contact>>();
builder.Services.AddScoped<IRepository<Users>, Repository<Users>>(); // Make sure to use Users repository
builder.Services.AddScoped<IRepository<ContactDTO>, Repository<ContactDTO>>();
builder.Services.AddScoped<IRepository<AppSettingsDTO>, Repository<AppSettingsDTO>>();
builder.Services.AddScoped<IRepository<LoginRequestDTO>, Repository<LoginRequestDTO>>();
builder.Services.AddTransient<IContactLogic, ContactLogic>();

// ? Token Generator
builder.Services.AddSingleton<TokenService>();

// ? Add Swagger with JWT Auth support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RealState API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter **JWT token** here without the `Bearer` prefix.\r\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4i... "
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "https://baljinderkanghomes.myskillwork.com"
              )
              .AllowAnyMethod()
              .AllowAnyHeader();
              
    });
});



var app = builder.Build();

// ? Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Optional: If you want to automatically configure Swagger UI to prompt for a token
        options.DefaultModelsExpandDepth(-1); // Hide the models section to focus on the API
    });
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthentication(); // ?? Must be before Authorization
app.UseAuthorization();



app.UseRouting();



app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
