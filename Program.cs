using AuthApi.UnitOfWork;
using AuthApi.Data;
using AuthApi.Repositories;
using AuthApi.Configurations;
using AuthApi.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using AuthApi.Features.Users.Commands.DeleteUser;
using AuthApi.Features.Users.Commands.RegisterUser;
using AuthApi.Features.Users.Commands.UpdateUser;
using AuthApi.Features.Users.Queries.GetAllUsers;
using AuthApi.Features.Users.Queries.GetUserById;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// 📌 appsettings.json'u Doğru Yükle
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// 📌 `JwtConfig` Ayarlarını Yükle
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtConfig>>().Value);

// 📌 Controller'ları ekle
builder.Services.AddControllers();

// 📌 Veritabanı Bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 📌 Identity Konfigürasyonu ve Roller
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 📌 MediatR Servisini Ekle (11.0.0 sürümüne uygun)
builder.Services.AddMediatR(typeof(Program).Assembly);

// 📌 Query & Command Handler'larını Kaydet
builder.Services.AddScoped<IRequestHandler<GetUserByIdQueryRequest, GetUserByIdQueryResponse>, GetUserByIdQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetAllUsersQueryRequest, GetAllUsersQueryResponse>, GetAllUsersQueryHandler>();
builder.Services.AddScoped<IRequestHandler<RegisterUserCommandRequest, RegisterUserCommandResponse>, RegisterUserCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateUserCommandRequest, UpdateUserCommandResponse>, UpdateUserCommandHandler>();
builder.Services.AddScoped<IRequestHandler<DeleteUserCommandRequest, DeleteUserCommandResponse>, DeleteUserCommandHandler>();

// 📌 Repository ve Servisleri Kaydet
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 📌 UnitOfWork Servisini Kaydet
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 📌 JWT Authentication Konfigürasyonu
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
if (string.IsNullOrEmpty(jwtConfig?.Secret))
{
    throw new Exception("🚨 JWT Secret is missing in appsettings.json");
}
var key = Encoding.UTF8.GetBytes(jwtConfig.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidateLifetime = true
    };
});

// 📌 CORS Ayarları
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// 📌 Swagger UI + JWT Yetkilendirme Konfigürasyonu
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthApi", Version = "v1" });

    // 🔑 JWT Authentication için Swagger Konfigürasyonu
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT tokenınızı aşağıdaki formatta girin: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// 📌 Varsayılan Roller ve Admin Kullanıcıyı Oluştur
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 📌 Rolleri Kontrol Et ve Yoksa Ekle
    string[] roles = new[] { "Manager", "TeamLeader", "Staff" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 📌 Varsayılan Bir Manager Kullanıcısı Oluştur
    var adminUser = await userManager.FindByEmailAsync("admin@company.com");
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = "admin@company.com",
            Email = "admin@company.com",
            FullName = "Admin User",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newAdmin, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Manager");
        }
    }
}

// 📌 Middleware Konfigürasyonu
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
