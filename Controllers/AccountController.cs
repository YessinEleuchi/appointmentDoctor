using AppointmentDoctor.Models;
using AppointmentDoctor.DTO;
using AppointmentDoctor.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 EmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }
        [HttpPost("register/doctor")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RegisterDoctor([FromForm] RegisterDoctor model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string imagePath = null;

            // Vérifier et sauvegarder l'image
            if (model.ProfileImage != null)
            {
                var fileExtension = Path.GetExtension(model.ProfileImage.FileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    return BadRequest("Format d'image non valide. Les formats autorisés sont : .jpg, .jpeg, .png.");
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/doctors");
                Directory.CreateDirectory(uploadsFolder); // Crée le dossier s'il n'existe pas
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                imagePath = $"/images/doctors/{fileName}";
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Adress = model.Adress,
                PhoneNumber = model.PhoneNumber,
                Speciality = model.Speciality,
                Fees = model.Fees,
                Experience = model.Experience,
                Gender = model.Gender,
                ProfileImagePath = imagePath
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "doctor");

            // Envoi du lien de confirmation par email
            await SendConfirmationEmail(user.Email);

            return Ok(new
            {
                message = $"Enregistrement réussi. Un email de confirmation a été envoyé à {model.Email}.",
                username = model.Username,
                profileImage = imagePath
            });
        }
        // Méthode pour enregistrer un admin
        [HttpPost("AdminCreateAdminUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminCreateAdminUser(RegisterUserDTO registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifiez si l'utilisateur existe déjà
            var userExists = await _userManager.FindByNameAsync(registerUser.Username);
            if (userExists != null)
            {
                return BadRequest("Cet utilisateur existe déjà.");
            }

            // Créer un nouvel utilisateur admin
            var newUser = new ApplicationUser
            {
                UserName = registerUser.Username,
                Email = registerUser.Email,
                PhoneNumber = registerUser.PhoneNumber,
                Adress = registerUser.Adress,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Gender = registerUser.Gender,
                Age = registerUser.Age,
            };

            var createResult = await _userManager.CreateAsync(newUser, registerUser.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors);
            }

            // Assigner le rôle 'admin'
            var defaultRole = "admin";
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                return BadRequest("Le rôle 'admin' n'existe pas.");
            }

            await _userManager.AddToRoleAsync(newUser, defaultRole);

            // Envoi du lien de confirmation par email
            await SendConfirmationEmail(newUser.Email);

            return Ok($"Utilisateur {registerUser.Username} avec le rôle {defaultRole} créé avec succès.");
        }

        // Méthode pour enregistrer un utilisateur "patient"
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExists = await _userManager.FindByNameAsync(registerUser.Username);
            if (userExists != null)
            {
                return BadRequest("Cet utilisateur existe déjà.");
            }

            var applicationUser = new ApplicationUser
            {
                UserName = registerUser.Username,
                Email = registerUser.Email,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Adress = registerUser.Adress,
                PhoneNumber = registerUser.PhoneNumber,
                Age = registerUser.Age,
                Gender = registerUser.Gender,
            };

            var result = await _userManager.CreateAsync(applicationUser, registerUser.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            string roleToAssign = "patient";
            if (!await _roleManager.RoleExistsAsync(roleToAssign))
            {
                return BadRequest("Le rôle 'patient' n'existe pas.");
            }

            await _userManager.AddToRoleAsync(applicationUser, roleToAssign);

            // Envoi du lien de confirmation par email
            await SendConfirmationEmail(applicationUser.Email);

            return Ok(new
            {
                message = $"Inscription réussie. Un email de confirmation a été envoyé à {registerUser.Email}.",
                username = registerUser.Username,
                role = roleToAssign
            });
        }

        // Méthode pour envoyer un lien de confirmation par email
        private async Task<IActionResult> SendConfirmationEmail(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest("L'email est déjà confirmé.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);

            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = user.Id, token = encodedToken },
                Request.Scheme);

            // Envoi de l'email de confirmation
            await _emailService.SendEmailAsync(userEmail, "Confirmer votre email", $"Cliquez sur ce lien pour confirmer votre email : {confirmationLink}");

            return Ok("Email de confirmation envoyé.");
        }





        // Méthode pour confirmer l'email
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Demande de confirmation de l'email invalide.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Décoder le token
            string decodedToken = HttpUtility.UrlDecode(token);

            // Tenter de confirmer l'email avec le token
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                return BadRequest("La confirmation de l'email a échoué.");
            }

            return Ok("Email confirmé avec succès.");
        }







    }
}