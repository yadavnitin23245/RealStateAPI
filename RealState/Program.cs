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

// -------------------- JWT Configuration --------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

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

builder.Services.AddAuthorization();

// -------------------- DB Context --------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// -------------------- Repositories & Logic Layer --------------------
builder.Services.AddScoped<IRepository<Contact>, Repository<Contact>>();
builder.Services.AddScoped<IRepository<Users>, Repository<Users>>();
builder.Services.AddScoped<IRepository<ContactStatDTO>, Repository<ContactStatDTO>>();
builder.Services.AddScoped<IRepository<ContactDTO>, Repository<ContactDTO>>();

builder.Services.AddScoped<IRepository<AppSettingsDTO>, Repository<AppSettingsDTO>>();
builder.Services.AddScoped<IRepository<LoginRequestDTO>, Repository<LoginRequestDTO>>();
builder.Services.AddTransient<IContactLogic, ContactLogic>();

builder.Services.AddSingleton<TokenService>();

// -------------------- Swagger Configuration --------------------
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
        Description = "Enter JWT token here without the 'Bearer' prefix."
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://baljinderkanghomes.myskillwork.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// -------------------- Middleware Pipeline --------------------

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Optional: Better error info during dev
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// ?? FIX: These must be in this order
app.UseRouting();            // 1. Routing comes first
app.UseAuthentication();     // 2. Authentication middleware
app.UseAuthorization();      // 3. Then Authorization middleware

app.MapControllers();

app.Run();
