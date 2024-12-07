using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AppointmentDoctor.DTO;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Enregistrement d'un patient (via formulaire normal)
        [HttpPost("RegisterPatient")]
        public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientDTO registerPatientDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByNameAsync(registerPatientDTO.Username);
            if (existingUser != null)
            {
                return BadRequest("L'utilisateur existe déjà.");
            }

            var patient = new Patient
            {
                UserName = registerPatientDTO.Username,
                Email = registerPatientDTO.Email,
                Address = registerPatientDTO.Address,
                MedicalHistory = registerPatientDTO.MedicalHistory
            };

            var result = await _userManager.CreateAsync(patient, registerPatientDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(patient, "patient");
                return Ok(new { Message = "Patient enregistré avec succès" });
            }

            return BadRequest(result.Errors);
        }

        // Enregistrement d'un docteur (réservé à l'admin)
        [HttpPost("RegisterDoctor")]
        public async Task<IActionResult> RegisterDoctor([FromBody] RegisterDoctorDTO registerDoctorDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByNameAsync(registerDoctorDTO.Username);
            if (existingUser != null)
            {
                return BadRequest("L'utilisateur existe déjà.");
            }

            var doctor = new Doctor
            {
                UserName = registerDoctorDTO.Username,
                Email = registerDoctorDTO.Email,
                Address = registerDoctorDTO.Address,
                Specialty = registerDoctorDTO.Specialty
            };

            var result = await _userManager.CreateAsync(doctor, registerDoctorDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(doctor, "doctor");
                return Ok(new { Message = "Docteur enregistré avec succès" });
            }

            return BadRequest(result.Errors);
        }

        // Enregistrement d'un administrateur (réservé à l'admin)
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDTO registerAdminDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByNameAsync(registerAdminDTO.Username);
            if (existingUser != null)
            {
                return BadRequest("L'utilisateur existe déjà.");
            }

            var admin = new ApplicationUser
            {
                UserName = registerAdminDTO.Username,
                Email = registerAdminDTO.Email,
                Address = registerAdminDTO.Address
            };

            var result = await _userManager.CreateAsync(admin, registerAdminDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "admin");
                return Ok(new { Message = "Administrateur enregistré avec succès" });
            }

            return BadRequest(result.Errors);
        }

        // Connexion d'un utilisateur (exemple de login)
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(loginDTO.Username);
            if (user == null)
            {
                return Unauthorized("Utilisateur ou mot de passe incorrect");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!result)
            {
                return Unauthorized("Utilisateur ou mot de passe incorrect");
            }

            // Vous pouvez ajouter ici un jeton JWT si nécessaire
            return Ok(new { Message = "Connexion réussie" });
        }
    }
}
