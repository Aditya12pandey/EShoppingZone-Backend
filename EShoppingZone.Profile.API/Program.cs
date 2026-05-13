using EShoppingZone.Profile.API.Data;
using EShoppingZone.Profile.API.Helpers;
using EShoppingZone.Profile.API.Repositories;
using EShoppingZone.Profile.API.Services;
using EShoppingZone.Profile.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ProfileDb");

// Render-specific: Use DATABASE_URL if available (postgres:// format)
var databaseUrl = builder.Configuration["DATABASE_URL"];
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userPass = Uri.UnescapeDataString(uri.UserInfo);
    var colonIndex = userPass.IndexOf(':');
    
    var connBuilder = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Database = uri.AbsolutePath.Trim('/'),
        Username = colonIndex > 0 ? userPass.Substring(0, colonIndex) : userPass,
        Password = colonIndex > 0 ? userPass.Substring(colonIndex + 1) : string.Empty,
        SslMode = Npgsql.SslMode.Require,
        TrustServerCertificate = true,
        MaxPoolSize = 5
    };
    connectionString = connBuilder.ToString();
}
else
{
    // Fallback to individual components
    var dbHost = builder.Configuration["DB_HOST"];
    if (!string.IsNullOrEmpty(dbHost))
    {
        var connBuilder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = dbHost,
            Port = int.Parse(builder.Configuration["DB_PORT"] ?? "5432"),
            Database = builder.Configuration["DB_NAME"],
            Username = builder.Configuration["DB_USER"],
            Password = builder.Configuration["DB_PASSWORD"],
            SslMode = Npgsql.SslMode.Require,
            TrustServerCertificate = true,
            MaxPoolSize = 10
        };
        connectionString = connBuilder.ToString();
    }
}

if (string.IsNullOrEmpty(connectionString))
{
    connectionString = "Host=localhost;Database=dummy;";
}

builder.Services.AddDbContext<ProfileDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<JwtHelper>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EShoppingZone — Profile API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
            {
                statusCode = 400,
                message = "Validation failed.",
                errors
            });
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try {
        var db = scope.ServiceProvider.GetRequiredService<ProfileDbContext>();
        var creator = db.Database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>();
        try { creator.CreateTables(); } catch { /* Tables may already exist */ }
    } catch (Exception ex) {
        Console.WriteLine("DB Init Error: " + ex.Message);
    }
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Profile API v1"));

if (app.Environment.IsDevelopment())
{
    // development only stuff here
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();