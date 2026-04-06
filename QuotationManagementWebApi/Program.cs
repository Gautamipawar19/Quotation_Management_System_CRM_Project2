using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuotationManagementWebApi.Application.Handlers.Quotes;
using QuotationManagementWebApi.DataSeeder;
using QuotationManagementWebApi.Infrastructure.Data;
using QuotationManagementWebApi.Infrastructure.Repositories.Implementation;
using QuotationManagementWebApi.Infrastructure.Repositories.Interfaces;
using QuotationManagementWebApi.Middleware;
using QuotationManagementWebApi.Services.Implementations;
using QuotationManagementWebApi.Services.Interfaces;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers + JSON Enum Support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Quotation Management Web API",
        Version = "v1",
        Description = "Quotation Management System API with JWT Authentication"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste only the raw JWT token here. Do not include the word Bearer.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// Database
builder.Services.AddDbContext<QuotationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();

// Services
builder.Services.AddScoped<ITaxCalculator, TaxCalculator>();
builder.Services.AddScoped<IQuoteNumberGenerator, QuoteNumberGenerator>();
builder.Services.AddScoped<IQuoteStatusValidator, QuoteStatusValidator>();
builder.Services.AddScoped<IQuotationService, QuotationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Handlers
builder.Services.AddScoped<GetAllQuotesQueryHandler>();
builder.Services.AddScoped<GetQuoteByIdQueryHandler>();
builder.Services.AddScoped<CreateQuoteCommandHandler>();
builder.Services.AddScoped<UpdateQuoteCommandHandler>();
builder.Services.AddScoped<DeleteQuoteCommandHandler>();
builder.Services.AddScoped<ChangeQuoteStatusCommandHandler>();
builder.Services.AddScoped<GetQuoteAnalyticsQueryHandler>();

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT Configuration
var jwtSection = builder.Configuration.GetSection("Jwt");

var keyString = jwtSection["Key"]
    ?? throw new InvalidOperationException("JWT Key is missing in appsettings.json.");

var issuer = jwtSection["Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing in appsettings.json.");

var audience = jwtSection["Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing in appsettings.json.");

var key = Encoding.UTF8.GetBytes(keyString);

// Authentication
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
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<QuotationDbContext>();
    await DbSeeder.SeedAsync(dbContext);
}

// Swagger Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "QuotationManagementWebApi v1");
    });
}

// Middleware Pipeline
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

await app.RunAsync();