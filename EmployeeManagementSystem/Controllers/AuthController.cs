using EmployeeManagement.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }





        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Validate input
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest($"Role '{model.Role}' does not exist");

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                // Create Employee record for Employee role
                if (model.Role == "Employee")
                {
                    if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) ||
                        string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.NationalId) ||
                        model.Age < 18 || model.Age > 100)
                    {
                        await _userManager.DeleteAsync(user);
                        return BadRequest("Invalid employee details");
                    }

                    var employee = new Employee
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        NationalId = model.NationalId,
                        Age = model.Age,
                        UserId = user.Id,
                        SignatureUrl = model.SignatureUrl

                    };
                    await _unitOfWork.Employees.AddAsync(employee);
                    await _unitOfWork.CompleteAsync();
                }

                return Ok("User registered successfully.");
            }

            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    employeeId = (await _userManager.FindByEmailAsync(model.Email)).Id,
                    role= userRoles.FirstOrDefault(),

                });
            }

            return Unauthorized("Invalid email or password.");
        }

    }
}
