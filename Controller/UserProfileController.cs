using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using EarthquakeReporter.Services; 
namespace EarthquakeReporter.Controllers
{
    [ApiController]
    [Route("api/userprofile")]
    [Authorize] // Ensure the controller requires authorization
    public class UserProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            // Log all claims for debugging
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            // Check for user ID claim
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "User ID not found in token." });
            }

            // Fetch user profile from the service
            var userProfile = await _userService.GetUserProfileAsync(userId);
            if (userProfile == null)
            {
                return NotFound(new { success = false, message = "User profile not found." });
            }

            return Ok(new { success = true, profile = userProfile });
        }
    }
}
