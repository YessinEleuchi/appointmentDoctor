using AppointmentDoctor.DTO;
using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updateUser)
        {
            // 1. Extraire l'identifiant de l'utilisateur à partir du token JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            // 2. Récupérer l'utilisateur dans la base de données
            var user = await _userManager.FindByIdAsync(userId);
          

            // 3. Mise à jour des champs transmis (champs optionnels)
            if (!string.IsNullOrWhiteSpace(updateUser.Email))
            {
                user.Email = updateUser.Email;
            }

            if (!string.IsNullOrWhiteSpace(updateUser.PhoneNumber))
            {
                user.PhoneNumber = updateUser.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(updateUser.Adress))
            {
                user.Adress = updateUser.Adress;
            }

            if (!string.IsNullOrWhiteSpace(updateUser.FirstName))
            {
                user.FirstName = updateUser.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(updateUser.LastName))
            {
                user.LastName = updateUser.LastName;
            }

            // 4. Gestion du mot de passe (si fourni)
            if (!string.IsNullOrWhiteSpace(updateUser.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(updateUser.CurrentPassword))
                {
                    return BadRequest(new { message = "Le mot de passe actuel est requis pour modifier le mot de passe." });
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, updateUser.CurrentPassword);
                if (!passwordCheck)
                {
                    return BadRequest(new { message = "Le mot de passe actuel est incorrect." });
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, updateUser.CurrentPassword, updateUser.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest(new { message = "Erreur lors de la modification du mot de passe.", errors = passwordChangeResult.Errors });
                }
            }

            // 5. Sauvegarder les modifications dans la base de données
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfileDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { error = "no user found with this id" });
            }

            return Ok(UserDTO.FromApplicationUser(user));
        }
    }
}
