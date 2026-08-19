using Microsoft.EntityFrameworkCore;
using Kipas.Personel.API.Data;
using Kipas.Personel.API.Interfaces;
using Kipas.Personel.API.Repositories;
using Kipas.Personel.API.Services;
using Kipas.Personel.API.Middleware;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.OpenApi;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<
        ApiDocumentTransformer>();

    options.AddOperationTransformer<
        AuthOperationTransformer>();
});


var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Veritabanı bağlantı bilgisi bulunamadı.");
}

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(connectionString));

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT gizli anahtarı bulunamadı.");
}

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT gizli anahtarı güvenlik amacıyla en az 32 byte olmalıdır.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "JWT issuer bilgisi bulunamadı.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT audience bilgisi bulunamadı.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    
        .AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.Zero
        };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userIdValue =
                context.Principal?
                    .FindFirst(
                        ClaimTypes.NameIdentifier)?
                    .Value;

            var tokenRole =
                context.Principal?
                    .FindFirst(
                        ClaimTypes.Role)?
                    .Value;

            if (!int.TryParse(
                    userIdValue,
                    out var userId) ||
                string.IsNullOrWhiteSpace(tokenRole))
            {
                context.Fail(
                    "Kullanıcı kimliği doğrulanamadı.");

                return;
            }

            var dbContext =
                context.HttpContext
                    .RequestServices
                    .GetRequiredService<
                        ApplicationDbContext>();

            var currentUser =
                await dbContext.Users
                    .AsNoTracking()
                    .Where(user =>
                        user.Id == userId)
                    .Select(user => new
                    {
                        user.IsActive,
                        user.Role
                    })
                    .SingleOrDefaultAsync(
                        context.HttpContext
                            .RequestAborted);

            if (currentUser == null)
            {
                context.Fail(
                    "Kullanıcı bulunamadı.");

                return;
            }

            if (!currentUser.IsActive)
            {
                context.Fail(
                    "Kullanıcı hesabı aktif değil.");

                return;
            }

            if (!string.Equals(
                    currentUser.Role,
                    tokenRole,
                    StringComparison.Ordinal))
            {
                context.Fail(
                    "Kullanıcı rolü değişmiş.");

                return;
            }
        },

        OnChallenge = async context =>
{
    context.HandleResponse();

    context.Response.StatusCode =
        StatusCodes.Status401Unauthorized;

    context.Response.ContentType =
        "application/json";

    await context.Response.WriteAsJsonAsync(
        new
        {
            success = false,
            message =
                "Kimlik doğrulaması gerekli veya token geçersiz."
        });
},

        OnForbidden = async context =>
        {
            context.Response.StatusCode =
                StatusCodes.Status403Forbidden;

            context.Response.ContentType =
                "application/json";

            await context.Response.WriteAsJsonAsync(
                new
                {
                    success = false,
                    message =
                        "Bu işlem için yetkiniz bulunmamaktadır."
                });
        }


    };

});
   
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularDevelopment",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.AddPolicy(
        "AuthLimit",
        httpContext =>
            RateLimitPartition
                .GetFixedWindowLimiter(
                    partitionKey:
                        httpContext.Connection
                            .RemoteIpAddress?
                            .ToString()
                        ?? "unknown",

                    factory: _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,

                            Window =
                                TimeSpan.FromMinutes(1),

                            QueueLimit = 0,

                            AutoReplenishment = true
                        }));
});



builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await AdminSeeder.SeedAsync(
        app.Services,
        app.Configuration);
}

app.UseExceptionHandler();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle(
            "Kipaş Personel Yönetim Sistemi API");
    });
}



// app.UseHttpsRedirection();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AngularDevelopment");
}

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
