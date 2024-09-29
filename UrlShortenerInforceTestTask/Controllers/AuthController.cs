using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        private readonly IConfiguration _config;

        public AuthController(IAppUserRepository appUserRepository, IConfiguration config)
        {
            _appUserRepository = appUserRepository;
            _config = config;
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

            return RedirectToAction("ShortURLsTable", "Urls");
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var email = loginViewModel.EmailAddress;
            var password = loginViewModel.Password;

            var user = await _appUserRepository.FindUserByEmailAsync(email);

            if (user != null)
            {
                bool passwordCheck = PasswordHasher.HashPassword(loginViewModel.Password) == user.Password;
                if (passwordCheck)
                {
                    bool userRole = user.IsAdmin;
                    var claims = new List<Claim>() 
                    { 
                        new Claim("userId", user.Id.ToString()),
                        new Claim("email", email)                    
                    };
                    if (userRole)
                        claims.Add(new Claim("role", "admin"));
                    else
                        claims.Add(new Claim("role", "user"));

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));

                    var signingCred = new SigningCredentials(securityKey,
                        SecurityAlgorithms.HmacSha256);

                    var securityToken = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(20),
                        issuer: _config.GetSection("Jwt:Issuer").Value,
                        audience: _config.GetSection("Jwt:Audience").Value,
                        signingCredentials: signingCred);

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
                    HttpContext.Session.SetString("JWToken", tokenString);


                    return RedirectToAction("ShortURLsTable", "Urls");
                    
                }
                TempData["Error"] = "Wrong credentials. Please, check your password";
                return View(loginViewModel);
            }

            TempData["Error"] = "Wrong credentials. Please, check your email or password";
            return View(loginViewModel);
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("ShortURLsTable", "Urls");
        }

    }
}
