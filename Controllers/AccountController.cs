// AccountController.cs
using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AppointmentDoctor.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly AppDbContext _context;


        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, AppDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            _context = context;
        }

        [HttpPost("register/doctor")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RegisterDoctor(RegisterDoctor model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // Créez un utilisateur avec les détails fournis
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Adress = model.Adress,
                PhoneNumber = model.PhoneNumber,
                Speciality = model.Speciality,

            };

            // Créez l'utilisateur avec le mot de passe
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Assignez le rôle de professionnel médical
            await userManager.AddToRoleAsync(user, "doctor");

            return Ok($"Enregistrement réussi du Doctor : {model.Username}");
        }
        [HttpPost("AdminCreateAdminUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminCreateAdminUser(RegisterUserDTO registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifiez si l'utilisateur existe déjà
            var userExists = await userManager.FindByNameAsync(registerUser.Username);
            if (userExists != null)
            {
                return BadRequest("Cet utilisateur existe déjà.");
            }

            // Créez un nouvel utilisateur
            var newUser = new ApplicationUser
            {
                UserName = registerUser.Username,
                Email = registerUser.Email,
                PhoneNumber = registerUser.PhoneNumber,
                Adress = registerUser.Adress,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
            };

            var createResult = await userManager.CreateAsync(newUser, registerUser.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors);
            }

            // Assignez le rôle 'admin' par défaut
            var defaultRole = "admin";
            await userManager.AddToRoleAsync(newUser, defaultRole);

            return Ok($"Utilisateur {registerUser.Username} avec le rôle {defaultRole} créé avec succès.");
        }

        [HttpGet("SearchDoctors")]
        public async Task<IActionResult> SearchDoctors(string critere)
        {
            if (string.IsNullOrEmpty(critere))
            {
                return BadRequest(new { message = "Le critère de recherche est requis." });
            }

            // Récupérer tous les utilisateurs qui ont le rôle 'doctor'
            var doctors = await userManager.Users
                .Where(d => (d.FirstName.ToLower().Contains(critere.ToLower()) ||
                             d.LastName.ToLower().Contains(critere.ToLower()) ||
                             d.Speciality.ToLower().Contains(critere.ToLower())))
                .ToListAsync();

            // Liste pour stocker les médecins filtrés
            var doctorList = new List<object>();

            foreach (var doctor in doctors)
            {
                // Vérifier si l'utilisateur a le rôle 'doctor' en utilisant RoleManager
                var roles = await userManager.GetRolesAsync(doctor);
                var roleExists = roles.Contains("doctor");

                if (roleExists)
                {
                    doctorList.Add(new
                    {
                        doctor.FirstName,
                        doctor.LastName,
                        doctor.Speciality,
                        doctor.Email,
                        doctor.PhoneNumber
                    });
                }
            }

            if (!doctorList.Any())
            {
                return NotFound(new { message = "Aucun médecin trouvé avec ce critère." });
            }

            return Ok(doctorList);
        }

    }
}