using AppointmentDoctor.Mapping;
using AppointmentDoctor.Models.Reposotries.Interfaces;
using AppointmentDoctor.Models.Reposotries;
using AppointmentDoctor.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurer la base de donn�es
var cnx = builder.Configuration.GetConnectionString("dbcon");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(cnx));

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddAutoMapper(typeof(MedicalHistoryMappingProfile), typeof(AppointmentMappingProfile));
builder.Services.AddTransient<EmailService>();

builder.Services.AddScoped<IMedicalHistoryRepository, MedicalHistoryRepository>();

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
        builder.WithOrigins("http://localhost:5173")  // Frontend React
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Ajouter les services n�cessaires
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
            Gender = "male",
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

app.UseAuthentication(); // Activer l'authentification JWT
app.UseAuthorization(); // Activer l'autorisation des utilisateurs

app.MapControllers();
app.Run();