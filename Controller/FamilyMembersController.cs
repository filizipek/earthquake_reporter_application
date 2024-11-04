using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EarthquakeReporter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FamilyMembersController : ControllerBase
    {
        private readonly IFamilyMemberRepository _repository;

        public FamilyMembersController(IFamilyMemberRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetFamilyMembers()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized("User email is required.");
                }

                var members = await _repository.GetFamilyMembersByEmailAsync(userEmail);
                return Ok(members);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching family members.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddFamilyMember([FromBody] FamilyMemberDto memberDto)
        {
            if (memberDto == null)
            {
                return BadRequest("Member data is required.");
            }

            if (string.IsNullOrEmpty(memberDto.Name) || string.IsNullOrEmpty(memberDto.Surname) ||
                string.IsNullOrEmpty(memberDto.Province) || string.IsNullOrEmpty(memberDto.Country) ||
                string.IsNullOrEmpty(memberDto.UserEmail))
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                await _repository.AddFamilyMemberAsync(memberDto);
                return Ok("Family member added successfully.");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Maximum number of family members"))
            {
                return BadRequest(ex.Message); // This returns a 400 Bad Request with the error message
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Console.WriteLine($"Error adding family member: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
