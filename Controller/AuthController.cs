using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using EarthquakeReporter.Models;
using EarthquakeReporter.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EarthquakeReporter.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthController(ILoginRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Validate request
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) ||
                string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Surname) || string.IsNullOrEmpty(request.Province) ||
                string.IsNullOrEmpty(request.Country) || request.Birthday == default)
            {
                return BadRequest(new { success = false, message = "Invalid registration details" });
            }

            // Ensure the email is not already registered
            var userExists = await _repository.EmailExistsAsync(request.Email);
            if (userExists)
            {
                return BadRequest(new { success = false, message = "User already registered" });
            }

            // Create new user
            var user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                Password = request.Password, // Hash password inside repository
                Birthday = request.Birthday,
                Province = request.Province,
                Country = request.Country
            };

            var success = await _repository.RegisterUserAsync(user);

            if (success)
            {
                return Ok(new { success = true, message = "Registration successful" });
            }

            return BadRequest(new { success = false, message = "Registration failed" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Validate request
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { success = false, message = "Invalid login details" });
            }

            // Authenticate user
            var user = await _repository.AuthenticateUserAsync(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Invalid email or password" });
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new { success = true, token });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Ensure this key is set in your appsettings.json
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Ensure user.Id is not null
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // For JWT, the token is managed client-side.
            // Invalidate the token on the client-side by clearing it from local storage or cookies.
            return Ok(new { success = true, message = "Logged out successfully" });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Birthday { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
    }
}
