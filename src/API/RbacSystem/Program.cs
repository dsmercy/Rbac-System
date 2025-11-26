using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RbacSystem.Authentication;
using RbacSystem.Data;
using RbacSystem.Mappings;
using RbacSystem.Middleware;
using RbacSystem.Services;
using RbacSystem.Validators;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure PostgreSQL with EF Core
builder.Services.AddDbContext<RbacDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("RedisConnection");
    return ConnectionMultiplexer.Connect(configuration!);
});

// Register services
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IExportService, ExportService>();

// Configure JWT Settings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure Swagger/OpenAPI
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "RBAC System API",
//        Version = "v1",
//        Description = "Role-Based Access Control (RBAC) System API with comprehensive user, group, role, and permission management",
//        Contact = new OpenApiContact
//        {
//            Name = "RBAC System",
//            Email = "support@rbacsystem.com"
//        }
//    });

//    // Add JWT Authentication to Swagger
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RBAC System API V1");
//        c.RoutePrefix = string.Empty; // Set Swagger UI at the root
//    });
//}

// Seed the database with sample data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<RbacDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Starting database seeding...");
        await DataSeeder.SeedDataAsync(context);
        logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Use custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Log application startup
Log.Information("RBAC System API starting up...");

try
{
    app.Run();
    Log.Information("RBAC System API started successfully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "RBAC System API failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}