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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Les numéros de page et la taille doivent être supérieurs à zéro." });
            }

            var totalUsers = await _userManager.Users.CountAsync();

            var users = await _userManager.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDetails = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDetails.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Roles = roles
                });
            }

            return Ok(new
            {
                TotalUsers = totalUsers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Users = userDetails
            });
        }

        // Mettre à jour un utilisateur
        [HttpPut("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updateUser)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Utilisateur non trouvé." });
            }

            // Mise à jour des champs optionnels
            user.Email = string.IsNullOrWhiteSpace(updateUser.Email) ? user.Email : updateUser.Email;
            user.PhoneNumber = string.IsNullOrWhiteSpace(updateUser.PhoneNumber) ? user.PhoneNumber : updateUser.PhoneNumber;
            user.Adress = string.IsNullOrWhiteSpace(updateUser.Adress) ? user.Adress : updateUser.Adress;
            user.FirstName = string.IsNullOrWhiteSpace(updateUser.FirstName) ? user.FirstName : updateUser.FirstName;
            user.LastName = string.IsNullOrWhiteSpace(updateUser.LastName) ? user.LastName : updateUser.LastName;

            // Gestion du mot de passe
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

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new { message = "Erreur lors de la mise à jour de l'utilisateur.", errors = updateResult.Errors });
            }

            return Ok(new { message = "Utilisateur mis à jour avec succès." });
        }

        // Supprimer un utilisateur
        [HttpDelete("DeleteUser/{username}")]
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

        // Obtenir les détails du profil
        [HttpGet("GetProfileDetails")]
        [Authorize]
        public async Task<IActionResult> GetProfileDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "Utilisateur non trouvé." });
            }

            return Ok(UserDTO.FromApplicationUser(user));
        }

        // Obtenir tous les médecins
        [HttpGet("GetAllDoctors")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllDoctors()
        {
            // Récupérer tous les utilisateurs ayant le rôle 'doctor'
            var users = await _userManager.Users.ToListAsync();

            var doctorList = new List<object>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("doctor"))
                {
                    doctorList.Add(new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.PhoneNumber,
                        user.Speciality,
                        user.Adress
                    });
                }
            }

            if (!doctorList.Any())
            {
                return NotFound(new { message = "Aucun médecin trouvé." });
            }

            return Ok(doctorList);
        }

        // Obtenir les médecins par spécialité
        [HttpGet("GetDoctorsBySpeciality")]
        public async Task<IActionResult> GetDoctorsBySpeciality(string speciality)
        {
            if (string.IsNullOrEmpty(speciality))
            {
                return BadRequest(new { message = "La spécialité est requise pour effectuer cette recherche." });
            }

            // Récupérer les médecins correspondant à la spécialité
            var users = await _userManager.Users
                .Where(user => user.Speciality.ToLower().Contains(speciality.ToLower()))
                .ToListAsync();

            var doctorList = new List<object>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("doctor"))
                {
                    doctorList.Add(new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.PhoneNumber,
                        user.Speciality,
                        user.Adress
                    });
                }
            }

            if (!doctorList.Any())
            {
                return NotFound(new { message = "Aucun médecin trouvé avec cette spécialité." });
            }

            return Ok(doctorList);
        }


        [HttpGet("SearchDoctors")]
        public async Task<IActionResult> SearchDoctors(string critere)
        {
            if (string.IsNullOrEmpty(critere))
            {
                return BadRequest(new { message = "Le critère de recherche est requis." });
            }

            // Récupérer tous les utilisateurs qui ont le rôle 'doctor'
            var doctors = await _userManager.Users
                .Where(d => (d.FirstName.ToLower().Contains(critere.ToLower()) ||
                             d.LastName.ToLower().Contains(critere.ToLower()) ||
                             d.Speciality.ToLower().Contains(critere.ToLower())))
                .ToListAsync();

            // Liste pour stocker les médecins filtrés
            var doctorList = new List<object>();

            foreach (var doctor in doctors)
            {
                // Vérifier si l'utilisateur a le rôle 'doctor' en utilisant RoleManager
                var roles = await _userManager.GetRolesAsync(doctor);
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
