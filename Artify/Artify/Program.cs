/*using System.Text;
using Artify.Data; // Rad sa bazom podataka
using Artify.Models; // Modeli korisnički definisanih entiteta
using Microsoft.AspNetCore.Identity; // Upravljanje identitetima korisnika i ulogama
using Microsoft.EntityFrameworkCore; // Rad sa Entity Framework-om i bazama podataka
using Microsoft.IdentityModel.Tokens; // Validacija i kreiranje JWT tokena
using Microsoft.OpenApi.Models; // Swagger dokumentacija
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Artify.Interfaces;
using Artify.Repositories;



var builder = WebApplication.CreateBuilder(args); // Kreiranje aplikacije i konfiguracija buildera

    // Dodavanje servisa za rad sa bazom podataka koristeći konekcioni string
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    // Konfiguracija Identity sistema za korisnike i uloge
    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true; // Zahtevaj bar jednu cifru
        options.Password.RequiredLength = 8; // Minimalna dužina lozinke
        options.Password.RequireUppercase = true; // Zahtevaj veliko slovo
        options.Password.RequireLowercase = true; // Zahtevaj malo slovo
        options.Password.RequireNonAlphanumeric = false; // Ne zahteva specijalne karaktere
    })
        .AddEntityFrameworkStores<AppDbContext>() // Koristi bazu podataka za čuvanje korisnika
        .AddDefaultTokenProviders(); // Generisanje tokena (npr. za resetovanje lozinki)

    // Dodavanje autentifikacije preko JWT tokena
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "JwtBearer"; // Podrazumevani šematski način autentifikacije
        options.DefaultChallengeScheme = "JwtBearer"; // Šema za izazove ako korisnik nije autentifikovan
    })
    .AddJwtBearer("JwtBearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt"); // Učitavanje podešavanja za JWT iz konfiguracije
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Proverava da li je issuer validan
            ValidateAudience = true, // Proverava da li je audience validna
            ValidateLifetime = true, // Proverava da li token nije istekao
            ValidateIssuerSigningKey = true, // Validacija ključa za potpisivanje
            ValidIssuer = builder.Configuration["Jwt:Issuer"],            // Postavljanje očekivanog issuer-a
            ValidAudience = builder.Configuration ["Jwt:Audience"], // Postavljanje očekivane publike
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))// Ključ za validaciju
        };
    });

    // Konfiguracija CORS politike (pristup API-u iz frontenda)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Dopušten domen za frontend
                  .AllowAnyHeader() // Dopušta sve HTTP zaglavlja
                  .AllowAnyMethod(); // Dopušta sve metode (GET, POST, itd.)
        });
    });

    // Dodavanje servisa za kontrolere (MVC logika)
    builder.Services.AddControllers();

    builder.Services.AddIdentity<Korisnik, IdentityRole>(options =>
    {
        // Konfiguracija opcija za Identity (ako je potrebno)
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppDbContext>() // Ova linija je važna!
    .AddDefaultTokenProviders();



// Dodavanje Swagger-a za automatsku dokumentaciju API-a
builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Artify API", Version = "v1" }); // API specifikacije
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header, // Token se šalje u HTTP zaglavlju
            Description = "Unesite validan token", // Opis u Swagger-u
            Name = "Authorization", // Naziv zaglavlja
            Type = SecuritySchemeType.Http, // HTTP tip sigurnosnog sistema
            BearerFormat = "JWT", // Format tokena
            Scheme = "Bearer" // JWT autentifikacija
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
                Array.Empty<string>() // Zahtev za validaciju bez dodatnih ograničenja
            }
        });
    });

    // Kreiranje aplikacije (pipeline konfiguracija)
    var app = builder.Build();

    // Provera okruženja i dodavanje Swagger-a ako je u razvojnom modu
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(); // Dodaje Swagger JSON dokumentaciju
        app.UseSwaggerUI(); // Omogućava interaktivan UI za testiranje API-a
    }

    // Omogućava automatsko preusmeravanje na HTTPS
    app.UseHttpsRedirection();

    // Aktivacija CORS politike
    app.UseCors("AllowFrontend");

    // Dodavanje autentifikacije i autorizacije u pipeline
    app.UseAuthentication();
    app.UseAuthorization();

    // Mapiranje kontrolera na odgovarajuće rute
    app.MapControllers();

    // Pokretanje aplikacije
    app.Run();
*/




using System.Text;
using Artify.Data; // Rad sa bazom podataka
using Artify.Models; // Modeli korisničkih entiteta
using Microsoft.AspNetCore.Identity; // Upravljanje identitetima korisnika i ulogama
using Microsoft.EntityFrameworkCore; // Rad sa Entity Framework-om i bazama podataka
using Microsoft.IdentityModel.Tokens; // Validacija i kreiranje JWT tokena
using Microsoft.OpenApi.Models; // Swagger dokumentacija
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Artify.Interfaces;
using Artify.Repositories;

var builder = WebApplication.CreateBuilder(args); // Kreiranje aplikacije i konfiguracija buildera

// Dodavanje servisa za rad sa bazom podataka koristeći konekcioni string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Konfiguracija Identity sistema za korisnike i uloge
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

// Dodavanje autentifikacije preko JWT tokena
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// Konfiguracija CORS politike (pristup API-u iz frontenda)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Dodavanje servisa za kontrolere (MVC logika)
builder.Services.AddControllers();

// Registracija korisnički definisanih servisa (npr. Repozitorijuma)
builder.Services.AddScoped<IKorisnik, KorisnikRepository>();

// Dodavanje Swagger-a za automatsku dokumentaciju API-a
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Artify API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Unesite validan token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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

// Kreiranje aplikacije (pipeline konfiguracija)
var app = builder.Build();

// Provera okruženja i dodavanje Swagger-a ako je u razvojnom modu
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Omogućava automatsko preusmeravanje na HTTPS
app.UseHttpsRedirection();

// Aktivacija CORS politike
app.UseCors("AllowFrontend");

// Dodavanje autentifikacije i autorizacije u pipeline
app.UseAuthentication();
app.UseAuthorization();

// Mapiranje kontrolera na odgovarajuće rute
app.MapControllers();

// Pokretanje aplikacije
app.Run();
