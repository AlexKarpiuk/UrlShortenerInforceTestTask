using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrlShortenerInforceTestTask.Data;
using UrlShortenerInforceTestTask.Interfaces;
using UrlShortenerInforceTestTask.Models;
using UrlShortenerInforceTestTask.Utils;
using UrlShortenerInforceTestTask.ViewModels;

namespace UrlShortenerInforceTestTask.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAppUserRepository _appUserRepository;

        public AuthController(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            var firstName = registerViewModel.FirstName;
            var lastName = registerViewModel.LastName;
            var email = registerViewModel.EmailAddress;
            var password = PasswordHasher.HashPassword(registerViewModel.Password);

            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _appUserRepository.FindUserByEmailAsync(email);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var newUser = new AppUser()
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                Email = email,
                Password = password
            };

            var newUserResponse = _appUserRepository.Add(newUser);
            if (newUserResponse)
                _appUserRepository.Save();
            else
            {
                TempData["Error"] = "Password should contain at least one digit, special character and uppercase symbol";
                return View(registerViewModel);
            }

            return RedirectToAction("ShowURLsTable", "Urls");
        }

    }
}
