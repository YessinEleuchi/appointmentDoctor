using AppointmentDoctor.DTO;
using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        // Connexion d'un utilisateur
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            // Vérification des données d'entrée
            if (login == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Données invalides" });
            }

            // Récupération de l'utilisateur par email
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
            {
                return Unauthorized(new { message = "Identifiants invalides" });
            }

            // Création des claims pour le JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Ajout des rôles de l'utilisateur dans les claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Création du token JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:iss"],
                audience: _configuration["JWT:aud"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            // Retour du token et des informations
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                userName = user.UserName,
                email = user.Email,
                roles = userRoles
            });
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifiez si l'utilisateur existe déjà
            var user = await _userManager.FindByNameAsync(registerUser.Username);
            if (user != null)
            {
                return BadRequest("Cet utilisateur existe déjà.");
            }

            // Créer un nouvel utilisateur
            var applicationUser = new ApplicationUser
            {
                UserName = registerUser.Username,
                Email = registerUser.Email,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Adress = registerUser.Adress,
                PhoneNumber = registerUser.PhoneNumber,
            };

            // Créer l'utilisateur avec son mot de passe
            var result = await _userManager.CreateAsync(applicationUser, registerUser.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Assigner systématiquement le rôle "patient"
            string roleToAssign = "patient";
            if (!await _roleManager.RoleExistsAsync(roleToAssign))
            {
                return BadRequest("Le rôle 'patient' n'existe pas. Contactez l'administrateur.");
            }

            await _userManager.AddToRoleAsync(applicationUser, roleToAssign);

            return Ok(new
            {
                username = registerUser.Username,
                phonenumber = registerUser.PhoneNumber,
                adress = registerUser.Adress,
                firstName = registerUser.FirstName,
                lastName = registerUser.LastName,
                email = registerUser.Email,
                role = roleToAssign
            });
        }
        // Déconnexion d'un utilisateur (simulé, car JWT ne gère pas directement la déconnexion)
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // La déconnexion peut être gérée côté client (en supprimant le token)
            return Ok("Utilisateur déconnecté. (Le client doit supprimer le token JWT)");
        }

    }
}
