using EShoppingZone.Cart.API.Data;
using EShoppingZone.Cart.API.Repositories;
using EShoppingZone.Cart.API.Services;
using EShoppingZone.Cart.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CartDb");

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

builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    })
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EShoppingZone Cart API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter: Bearer {your JWT token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Auto-migration and manual schema fix
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
    db.Database.EnsureDeleted(); // CLEAN SLATE
    db.Database.EnsureCreated();
}

app.Run();