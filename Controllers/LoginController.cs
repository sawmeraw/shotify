using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ViewModels;

namespace Shotify.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly string _storedPassword;
        private readonly SymmetricSecurityKey _signingKey;
        public LoginController()
        {
            _storedPassword = Environment.GetEnvironmentVariable("LOGIN") ?? "default";
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT_SECRET_KEY env variable is not set.");
            }
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
        }

        [Route("/login", Name = "Login")]
        public IActionResult Index(string? ReturnUrl = null)
        {
            if(Request.Cookies.TryGetValue("AuthToken", out var incomingToken))
            {
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    var principal = handler.ValidateToken(incomingToken, new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = _signingKey,
                        ClockSkew = TimeSpan.Zero
                    }, out _);

                    return RedirectToRoute("HomePage");
                }
                catch (Exception)
                {
                    return RedirectToRoute("Login");
                }
            }
            return View(new LoginViewModel
            {
                ReturnUrl = ReturnUrl,
            });
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Post(LoginViewModel dto, [FromQuery] string? ReturnUrl = null)
        {

            if (!ModelState.IsValid)
            {
                return View("Index", dto);
            }

            if(dto.Password != _storedPassword)
            {
                dto.ErrorMessage = "Invalid Password";
                return View("Index", dto);
            }

            var token = GenerateJwtToken();

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(12)
            });
            
            if(!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            
            return RedirectToRoute("HomePage");
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToRoute("Login");
        }

        private string GenerateJwtToken()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin"),
                new Claim(ClaimTypes.Role, "editor")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
