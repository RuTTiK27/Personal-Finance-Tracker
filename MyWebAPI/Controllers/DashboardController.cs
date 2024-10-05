using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UsersController.Controllers
{
    [ApiController]
    [Route("api/")]
    public class DashboardController : ControllerBase
    {
        // Example: A simple GET action 
        [HttpGet("dashboard")]
        public IActionResult GetDashboardData()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); // Returns 401
            }

            // Your logic to return dashboard data
            return Ok(new { message = "Welcome to the dashboard!" });
        }
    }
}
