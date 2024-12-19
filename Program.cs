using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using AppointmentDoctor.Models.Reposotries;
using AppointmentDoctor.Models.Reposotries.Interfaces;
using AppointmentDoctor.Mapping;
using AppointmentDoctor.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurer la base de donn�es
var cnx = builder.Configuration.GetConnectionString("dbcon");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(cnx));

// Ajouter les d�pendances des repositories et services
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IMedicalHistoryRepository, MedicalHistoryRepository>();
builder.Services.AddAutoMapper(typeof(MedicalHistoryMappingProfile), typeof(AppointmentMappingProfile));
builder.Services.AddTransient<EmailService>();

// Configurer Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configurer l'authentification avec JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["JWT:iss"],
        ValidAudience = builder.Configuration["JWT:aud"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configurer CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:5173") // Frontend React
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Ajouter Swagger avec configuration JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Appointment Doctor API",
        Version = "v1",
        Description = "API pour g�rer les rendez-vous m�dicaux."
    });

    // Ajouter la s�curit� JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// Configurer les contr�leurs avec options JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64; // Ajuster selon les besoins
    });

builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();

// Initialiser les r�les et l'utilisateur admin au d�marrage
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Cr�er les r�les 'admin', 'doctor', 'patient' si ils n'existent pas
    string[] roles = { "admin", "doctor", "patient" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Cr�er un utilisateur admin si il n'existe pas
    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@admin.com",
            Adress = "ain chikh rouhou",
            FirstName = "yessin",
            LastName = "eleuchi",
            PhoneNumber = "56123413",
            EmailConfirmed = true,
            Gender = "uuuuu",
        };
        var adminPassword = "Admin@123";
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "admin");
        }
    }
}

// Configurer le pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activer CORS
app.UseCors("AllowReactApp");
app.UseStaticFiles(); // Servir les fichiers statiques

app.UseAuthentication(); // Activer l'authentification JWT
app.UseAuthorization(); // Activer l'autorisation des utilisateurs

app.MapControllers();
app.Run();

