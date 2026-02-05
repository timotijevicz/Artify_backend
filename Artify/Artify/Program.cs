using System.Text;
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

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Identity (koristi tvoj Korisnik)
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

//
// ✅ KLJUČNO 1: Spreči cookie redirect na /Account/Login za API rute
//
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

//
// ✅ KLJUČNO 2: JWT kao default auth + challenge (da [Authorize] radi preko Bearer)
//
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

        // ✅ KLJUČNO: da Roles rade sa tvojim claim-om u tokenu
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

        // ✅ preporučeno: da NameIdentifier radi stabilno
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
    };
});

// Authorization
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Controllers
builder.Services.AddControllers();

// DI: Repozitorijumi/Servisi
builder.Services.AddScoped<IKorisnik, KorisnikRepository>();
builder.Services.AddScoped<IFavoriti, FavoritiRepository>();
builder.Services.AddScoped<IUmetnickoDelo, UmetnickoDeloRepository>();
builder.Services.AddScoped<IToken, TokenService>();

// Swagger + Bearer
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

// Seed role + test korisnici
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

    var umetnikUser = await CreateUserIfNotExists(userManager, "artist@artify.com", "Artist123!", "Test Umetnik", "Umetnik");

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// pomocna metoda
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
 