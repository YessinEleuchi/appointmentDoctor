using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Assigner un rôle à un utilisateur (admin uniquement)
        [HttpPost("AssignRole")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignRole([FromQuery] string username, [FromQuery] string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                return BadRequest("Le nom d'utilisateur et le rôle sont obligatoires.");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound($"Utilisateur '{username}' introuvable.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return BadRequest($"Le rôle '{role}' n'existe pas.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(role))
            {
                return BadRequest($"L'utilisateur '{username}' possède déjà le rôle '{role}'.");
            }

            // Supprimer les anciens rôles et attribuer le nouveau rôle
            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest("Échec de la suppression des anciens rôles.");
            }

            var addResult = await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
            {
                return BadRequest("Échec de l'attribution du rôle.");
            }

            return Ok($"Le rôle '{role}' a été attribué à l'utilisateur '{username}'.");
        }

        // Méthode pour créer les rôles au démarrage (si nécessaire)
        [HttpPost("CreateRoles")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateRoles()
        {
            string[] roleNames = { "admin", "doctor", "patient" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await _roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        return BadRequest($"Échec de la création du rôle '{roleName}'.");
                    }
                }
            }

            return Ok("Tous les rôles ont été créés avec succès.");
        }

        // Obtenir les rôles d'un utilisateur
        [HttpGet("GetUserRoles")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUserRoles([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Le nom d'utilisateur est obligatoire.");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound($"Utilisateur '{username}' introuvable.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { username, roles });
        }
    }
}
