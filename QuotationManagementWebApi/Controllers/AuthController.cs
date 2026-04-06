using Microsoft.AspNetCore.Mvc;
using QuotationManagementWebApi.DTOs.Requests;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "sakshi" && request.Password == "123")
            {
                var token = _authService.GenerateToken("sakshi", "SalesRep");
                return Ok(new { token });
            }
            if (request.Username == "gauri" && request.Password == "123")
            {
                var token = _authService.GenerateToken("gauri", "SalesRep");
                return Ok(new { token });
            }

            if (request.Username == "gautami" && request.Password == "123")
            {
                var token = _authService.GenerateToken("gautami", "SalesRep");
                return Ok(new { token });
            }

            if (request.Username == "manager" && request.Password == "123")
            {
                var token = _authService.GenerateToken("manager", "SalesManager");
                return Ok(new { token });
            }

            if (request.Username == "admin" && request.Password == "123")
            {
                var token = _authService.GenerateToken("admin", "Admin");
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid credentials." });
        }
    }
}