using System.Text;
using System.Text.Json.Serialization;
using Artify.Data;
using Artify.Models;
using Artify.Interfaces;
using Artify.Repositories;
using Artify.Token;
using Artify.GraphQL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Railway PORT binding (bitno za deploy)
// =======================
// Railway u praksi prosleđuje PORT (često 8080). Najsigurnije je da slušaš baš taj port.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// =======================
// Kestrel – forsiraj HTTP/1.1
// =======================
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(lo => { lo.Protocols = HttpProtocols.Http1; });
});

// =======================
// DbContext (AUTO: LocalDB -> SQL Server, ostalo -> MySQL)
// =======================
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("DefaultConnection nije podešen.");

var isSqlServerLocalDb =
    cs.Contains("(localdb)", StringComparison.OrdinalIgnoreCase) ||
    cs.Contains("mssqllocaldb", StringComparison.OrdinalIgnoreCase) ||
    cs.Contains("MSSQLLocalDB", StringComparison.OrdinalIgnoreCase);

if (isSqlServerLocalDb)
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(cs));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(cs, ServerVersion.AutoDetect(cs)));
}

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

    options.IncludeErrorDetails = builder.Environment.IsDevelopment();

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
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
    };

    if (builder.Environment.IsDevelopment())
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("❌ JWT auth FAILED");
                Console.WriteLine(ctx.Exception);
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                Console.WriteLine("✅ JWT token VALID");
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Console.WriteLine($"⚠️ JWT CHALLENGE: {ctx.Error} | {ctx.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    }
});

builder.Services.AddAuthorization();

// =======================
// CORS (dozvoli React domen)
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "https://artifyfrontend-production.up.railway.app",
                "http://localhost:3000",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        // Ako ikad budeš slao cookies: .AllowCredentials();
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
// GraphQL (HotChocolate)
// =======================
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddFiltering()
    .AddSorting();

// =======================
// DI
// =======================
builder.Services.AddScoped<IKorisnik, KorisnikRepository>();
builder.Services.AddScoped<IFavoriti, FavoritiRepository>();
builder.Services.AddScoped<IRecenzija, RecenzijaRepository>();
builder.Services.AddScoped<IPorudzbina, PorudzbinaRepository>();
builder.Services.AddScoped<IUmetnickoDelo, UmetnickoDeloRepository>();
builder.Services.AddScoped<INotifikacija, NotifikacijaRepository>();
builder.Services.AddScoped<IUmetnik, UmetnikRepository>();
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
// Migracije + Seed
// =======================
// Migracije možeš ostaviti u prod ako rade (kod tebe po logu rade).
// Seed ostaje samo u Development.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    if (app.Environment.IsDevelopment())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Korisnik>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

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
}

// =======================
// Middleware — redosled
// =======================

// Swagger uključen i u Production (da možeš da testiraš na Railway).
// Ako hoćeš da ga gasiš kasnije: stavi uslov preko ENV npr. ENABLE_SWAGGER=true.
app.UseSwagger();
app.UseSwaggerUI();

// Railway već terminira HTTPS, ali ovo obično ne smeta.
// Ako primetiš probleme sa OPTIONS redirectom, možeš ga držati samo u dev.
app.UseHttpsRedirection();

// Ove warninge ignoriši na Railway (wwwroot možda nemaš).
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontend");

// Catch-all preflight handler (pomaže kad browser šalje OPTIONS)
app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.NoContent())
   .RequireCors("AllowFrontend");

// Health + root da ne dobijaš 405 u browseru
app.MapGet("/", () => Results.Ok("Artify backend is running")).RequireCors("AllowFrontend");
app.MapGet("/health", () => Results.Ok("ok")).RequireCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();

// =======================
// Helper (seed samo za dev)
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