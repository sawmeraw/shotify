using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace Shotify.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        [Route("/login")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Post(LoginViewModel dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var expected = Environment.GetEnvironmentVariable("LOGIN");
            if(expected != null)
            {
                if(dto.Password != expected)
                {
                    dto.ErrorMessage = "Invalid Password";
                    return View(dto);
                }
            }
            
            if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
