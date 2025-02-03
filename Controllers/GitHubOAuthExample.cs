using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace JWTProtectedAPI.Controllers
{   

    
        [Route("auth")]
        [ApiController]
        public class GitAuthController : ControllerBase
        {
            // Trigger the GitHub OAuth flow
            [HttpGet("login")]
            public IActionResult Login()
            {
                // This will redirect the user to GitHub's login page
                return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "GitHub");
            }

            // GitHub redirects to this endpoint after authentication
            [HttpGet("signin-github")]
            public async Task<IActionResult> GitHubCallback()
            {
                // Get the OAuth token and use it to fetch user details
                var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (!authenticateResult.Succeeded)
                    return Unauthorized();

                var userClaims = authenticateResult.Principal.Claims;
                // You can now access user information (e.g., userClaims) and process it

                return Ok(new
                {
                    Message = "GitHub OAuth succeeded!",
                    Claims = userClaims
                });
            }
        }

}
