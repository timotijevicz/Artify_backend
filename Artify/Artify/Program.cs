using System.Text;
using System.Text.Json.Serialization;
using Artify.Data;
using Artify.Models;
using Artify.Interfaces;
using Artify.Repositories;
using Artify.Token;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Kestrel – forsiraj HTTP/1.1
// =======================
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(lo =>
    {
        lo.Protocols = HttpProtocols.Http1;
    });
});

// =======================
// DbContext
// =======================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// =======================
// Identity
// =======================
builder.Services.AddIdentity<Korisnik, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// =======================
// Cookie redirect FIX za API
// =======================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = ctx =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            ctx.Response.Redirect(ctx.RedirectUri);
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = ctx =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
            ctx.Response.Redirect(ctx.RedirectUri);
            return Task.CompletedTask;
        }
    };
});

// =======================
// JWT Authentication
// =======================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
        ),

        ClockSkew = TimeSpan.Zero,

        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
    };
});

builder.Services.AddAuthorization();

// =======================
// CORS
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// =======================
// Controllers + IgnoreCycles
// =======================
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// =======================
// DI
// =======================
builder.Services.AddScoped<IKorisnik, KorisnikRepository>();
builder.Services.AddScoped<IFavoriti, FavoritiRepository>();
builder.Services.AddScoped<IRecenzija, RecenzijaRepository>();
builder.Services.AddScoped<IPorudzbina, PorudzbinaRepository>();
builder.Services.AddScoped<IUmetnickoDelo, UmetnickoDeloRepository>();
builder.Services.AddScoped<IToken, TokenService>();

// =======================
// Swagger
// =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Artify API", Version = "v1" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Unesi: Bearer {token}"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// =======================
// Seed (roles + test korisnici)
// =======================
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Korisnik>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    string[] roles = { "Admin", "Umetnik", "Kupac" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    await CreateUserIfNotExists(userManager, "admin@artify.com", "Admin123!", "Admin Artify", "Admin");
    await CreateUserIfNotExists(userManager, "korisnik@artify.com", "Korisnik123!", "Obican Korisnik", "Kupac");

    var umetnikUser = await CreateUserIfNotExists(
        userManager,
        "artist@artify.com",
        "Artist123!",
        "Test Umetnik",
        "Umetnik"
    );

    if (umetnikUser != null)
    {
        var postoji = await dbContext.Umetnici.AnyAsync(u => u.KorisnikId == umetnikUser.Id);
        if (!postoji)
        {
            dbContext.Umetnici.Add(new Umetnik
            {
                KorisnikId = umetnikUser.Id,
                Biografija = "Test umetnik za razvoj.",
                Tehnika = "Digital art",
                Stil = "Modern",
                Specijalizacija = "Ilustracije",
                IsApproved = true,
                IsAvailable = true
            });

            await dbContext.SaveChangesAsync();
        }
    }
}

// =======================
// Middleware
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// wwwroot (ako postoji)
app.UseStaticFiles();

// ✅ Uploads folder (OVO TI JE FALILO)
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(builder.Environment.ContentRootPath, "Uploads")
//    ),
//    RequestPath = "/uploads"
//});

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// =======================
// Helper
// =======================
static async Task<Korisnik?> CreateUserIfNotExists(
    UserManager<Korisnik> userManager,
    string email,
    string password,
    string imeIPrezime,
    string role
)
{
    var user = await userManager.FindByEmailAsync(email);
    if (user != null) return user;

    user = new Korisnik
    {
        Email = email,
        UserName = email,
        ImeIPrezime = imeIPrezime,
        DatumRegistracije = DateTime.UtcNow
    };

    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded) return null;

    await userManager.AddToRoleAsync(user, role);
    return user;
}
