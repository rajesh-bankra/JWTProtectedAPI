using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTProtectedAPI.Controllers
{    

    namespace JwtAuthDemo.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class AuthController : ControllerBase
        {
            private readonly IConfiguration _configuration;

            public AuthController(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            [HttpPost("login")]
            public IActionResult Login([FromBody] LoginModel login)
            {
                // For demo purposes, you can use hardcoded credentials.
                // In real applications, validate the user with a database.
                if (login.Username == "string" && login.Password == "string")
                {
                    var claims = new[]
                     {
                        new Claim(ClaimTypes.Name, login.Username),
                        // Add any other claims as needed
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(20),
                        signingCredentials: creds
                    );

                    // Generate the token as a string
                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                    return Ok(new { token = jwtToken });

                }

                return Unauthorized();
            }

            [Authorize] // Only accessible by authenticated users
            [HttpGet("GetAuthorisedData")]
            public IActionResult GetAuthorisedData()
            {
                return Ok(new { message = "This is a secure route" });
            }
        }

        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}


