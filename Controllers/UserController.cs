using AppointmentDoctor.DTO;
using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Obtenir tous les utilisateurs avec leurs rôles
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Les numéros de page et la taille doivent être supérieurs à zéro." });
            }

            // Récupérer les utilisateurs avec pagination
            var users = await _userManager.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Récupérer les rôles pour chaque utilisateur
            var userDetails = new List<object>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDetails.Add(new
                {
                    user.Id,
                    user.UserName,
                    Roles = roles
                });
            }

            return Ok(userDetails);
        }

        // Mettre à jour un utilisateur
        [HttpPut("UpdateUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUser(string username, [FromBody] UpdateUserDTO updateUser)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Le nom d'utilisateur est requis." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "Utilisateur non trouvé." });
            }

            // Mettre à jour les informations de l'utilisateur
            user.Email = updateUser.Email ?? user.Email;
            user.UserName = updateUser.NewUsername ?? user.UserName;
            user.Adress = updateUser.Adress ?? user.Adress;
            user.PhoneNumber = updateUser.PhoneNumber ?? user.PhoneNumber;
            user.FirstName = updateUser.FirstName ?? user.FirstName;
            user.LastName = updateUser.LastName ?? user.LastName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new { message = "Erreur lors de la mise à jour de l'utilisateur.", errors = updateResult.Errors });
            }

            return Ok(new { message = "Utilisateur mis à jour avec succès." });
        }

        // Supprimer un utilisateur
        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Le nom d'utilisateur est requis." });
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "Utilisateur non trouvé." });
            }

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return BadRequest(new { message = "Erreur lors de la suppression de l'utilisateur.", errors = deleteResult.Errors });
            }

            return Ok(new { message = $"Utilisateur {username} supprimé avec succès." });
        }
    }
}
