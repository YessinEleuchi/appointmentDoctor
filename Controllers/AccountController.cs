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
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifiez si l'utilisateur existe déjà
            var user = await userManager.FindByNameAsync(registerUser.Username);
            if (user != null)
            {
                return BadRequest("Cet utilisateur existe déjà.");
            }

            // Créer un nouvel utilisateur
            var applicationUser = new ApplicationUser
            {
                UserName = registerUser.Username,
                Email = registerUser.Email
            };

            // Créer l'utilisateur avec son mot de passe
            var result = await userManager.CreateAsync(applicationUser, registerUser.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Assigner systématiquement le rôle "patient"
            string roleToAssign = "patient";
            if (!await roleManager.RoleExistsAsync(roleToAssign))
            {
                return BadRequest("Le rôle 'patient' n'existe pas. Contactez l'administrateur.");
            }

            await userManager.AddToRoleAsync(applicationUser, roleToAssign);

            return Ok(new
            {
                username = registerUser.Username,
                email = registerUser.Email,
                role = roleToAssign
            });
        }


        // Connexion d'un utilisateur
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByNameAsync(login.Username);
            if (user == null || !await userManager.CheckPasswordAsync(user, login.Password))
            {
                return Unauthorized("Identifiants invalides");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:iss"],
                audience: configuration["JWT:aud"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                username = login.Username
            });
        }

        // Assigner un rôle à un utilisateur (admin uniquement)
        [HttpPost("AssignRole")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignRole([FromQuery] string username, [FromQuery] string role)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé");
            }

            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return BadRequest("Rôle invalide");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            if (userRoles.Contains(role))
            {
                return BadRequest($"L'utilisateur {username} a déjà le rôle '{role}'");
            }

            // Si l'utilisateur possède déjà un rôle, le supprimer et ajouter le nouveau rôle
            var removeResult = await userManager.RemoveFromRolesAsync(user, userRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest("Échec de la suppression des anciens rôles");
            }

            await userManager.AddToRoleAsync(user, role);
            return Ok($"Le rôle '{role}' a été attribué à l'utilisateur {username}");
        }

        // Obtenir tous les utilisateurs avec leurs rôles
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            var users = userManager.Users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var userRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userRoles.Add(new { user.UserName, user.Email, Roles = roles });
            }

            return Ok(userRoles);
        }


        // Obtenir les rôles d'un utilisateur
        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles([FromQuery] string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound($"Utilisateur {username} introuvable.");
            }

            var roles = await userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        // Déconnexion d'un utilisateur (simulé, car JWT ne gère pas directement la déconnexion)
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // La déconnexion peut être gérée côté client (en supprimant le token)
            return Ok("Utilisateur déconnecté. (Le client doit supprimer le token JWT)");
        }

        // Méthode pour créer les rôles au démarrage (si nécessaire)
        [HttpPost("CreateRoles")]
        [Authorize(Roles = "admin")] // Seul un admin peut créer les rôles
        public async Task<IActionResult> CreateRoles()
        {
            string[] roleNames = { "admin", "doctor", "patient" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }

            return Ok("Rôles créés avec succès.");
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



        [HttpPut("UpdateUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUser(string username, UpdateUserDTO updateUser)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Mettre à jour les informations de l'utilisateur
            user.Email = updateUser.Email ?? user.Email;
            user.UserName = updateUser.NewUsername ?? user.UserName;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            return Ok("Utilisateur mis à jour avec succès.");
        }

        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return BadRequest(deleteResult.Errors);
            }

            return Ok($"Utilisateur {username} supprimé avec succès.");
        }


    }
}