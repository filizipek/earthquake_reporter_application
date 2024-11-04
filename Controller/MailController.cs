namespace YourProject.Controllers
{
    using EarthquakeReporter.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public MailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] EmailRequest request)
        {
            await _emailService.SendEmailAsync(request.ToAddress, request.Subject, request.Body);
            return Ok("Email sent successfully.");
        }
    }

    public class EmailRequest
    {
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
