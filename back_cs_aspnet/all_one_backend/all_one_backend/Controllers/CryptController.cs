using all_one_backend.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace all_one_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CryptController : ControllerBase
    {
        private readonly IJwtHandler _jwtHandler;

        public CryptController(IJwtHandler jwtHandler)
        {
            _jwtHandler = jwtHandler;
        }

        [Authorize]
        [HttpGet("decodeToken_generatedAsJWT")]
        public async Task<IActionResult> DecodeReceived([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                var claims = _jwtHandler.DecodeToken(token);
                if (claims == null)
                    return BadRequest("Invalid Token");
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Something went the wrong way: ({ex.Message})");
            }
        }
    }
}
