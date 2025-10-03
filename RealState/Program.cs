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
using Microsoft.OpenApi.Models;
using System.Text;

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

// -------------------- CORS --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173", // local dev
            "https://baljinderkanghomes.myskillwork.com" ,// live frontend,
            "https://bkhome.ca",
            "http://www.bkhome.ca",
            "https://www.bkhome.ca"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

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

builder.Services.AddScoped<IRepository<PaymentFrequency>, Repository<PaymentFrequency>>();
builder.Services.AddScoped<IRepository<PaymentFrequencyDTO>, Repository<PaymentFrequencyDTO>>();
builder.Services.AddScoped<IRepository<Amortizationfrequency>, Repository<Amortizationfrequency>>();
builder.Services.AddScoped<IRepository<AmortizationfrequencyDTO>, Repository<AmortizationfrequencyDTO>>();

builder.Services.AddScoped<IRepository<canadacities>, Repository<canadacities>>();
builder.Services.AddScoped<IRepository<canadacitiesDTO>, Repository<canadacitiesDTO>>();

builder.Services.AddTransient<IContactLogic, ContactLogic>();

builder.Services.AddSingleton<TokenService>();

// -------------------- Swagger --------------------
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
        Description = "Enter JWT token without 'Bearer ' prefix"
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

var app = builder.Build();

// -------------------- Middleware Pipeline --------------------

if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
    });
}
else
{
    app.UseSwagger(); // Optional: remove if not needed in production
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowFrontend"); // ?? Apply CORS after routing but before auth

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
