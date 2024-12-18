using AppointmentDoctor.DTO;
using AppointmentDoctor.Models;
using AppointmentDoctor.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            EmailService emailService, IConfiguration configuration,
            AppDbContext context)
        {
            _userManager = userManager;
            _emailService = emailService;
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

            // Vérification si l'email est confirmé
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Unauthorized(new { message = "Veuillez confirmer votre email avant de vous connecter." });
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

        // Fonction pour la demande de réinitialisation de mot de passe
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            // Vérification que l'email est bien fourni
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("L'email est requis.");

            // Recherche de l'utilisateur par son email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("Utilisateur introuvable.");

            // Génération du token de réinitialisation du mot de passe
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Envoi de l'email avec le token
            var messageBody = $"Pour réinitialiser votre mot de passe, utilisez ce token : {token}";

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Réinitialisation du mot de passe", messageBody);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de l'envoi de l'email: " + ex.Message);
            }

            return Ok("Email de réinitialisation envoyé.");
        }

        // Fonction pour la réinitialisation du mot de passe
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            // Vérification que tous les champs sont remplis
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
                return BadRequest("Tous les champs sont requis.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("Utilisateur introuvable.");

            // Réinitialisation du mot de passe en utilisant le token fourni
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
            {
                // Retourne les erreurs si la réinitialisation échoue
                return BadRequest(result.Errors);
            }

            return Ok("Mot de passe réinitialisé avec succès.");
        }
    }
}
