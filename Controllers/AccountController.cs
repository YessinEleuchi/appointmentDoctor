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


        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration , AppDbContext context)
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

            // Vérifiez que la spécialité existe dans la base de données
            var speciality = await _context.Specialities
                .FirstOrDefaultAsync(s => s.Name == model.Specialty);
            if (speciality == null)
            {
                return BadRequest("La spécialité spécifiée n'existe pas.");
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
                SpecialtyId = speciality.Id, // Associer l'ID de la spécialité

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

            // Validez le rôle
            var roleToAssign = string.IsNullOrEmpty(registerUser.Role) ? "admin" : registerUser.Role.ToLower();
            if (roleToAssign != "admin")
            {
                return BadRequest("Le rôle doit être 'admin'.");
            }

            // Créez un nouvel utilisateur
            var newUser = new ApplicationUser
            {
                UserName = registerUser.Username,
                Email = registerUser.Email
            };

            var createResult = await userManager.CreateAsync(newUser, registerUser.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors);
            }

            // Assignez le rôle 'admin'
            await userManager.AddToRoleAsync(newUser, roleToAssign);

            return Ok($"Utilisateur {registerUser.Username} avec le rôle {roleToAssign} créé avec succès.");
        }



        


    }
}