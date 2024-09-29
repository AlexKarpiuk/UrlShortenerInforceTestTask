using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using UrlShortenerInforceTestTask.Interfaces;
using UrlShortenerInforceTestTask.Models;
using UrlShortenerInforceTestTask.Repositories;
using UrlShortenerInforceTestTask.Utils;
using UrlShortenerInforceTestTask.ViewModels;

namespace UrlShortenerInforceTestTask.Controllers
{
    public class UrlsController : Controller
    {
        private readonly ILogger<UrlsController> _logger;
        private readonly IUrlsRepository _urlsRepository;
        private readonly IAppUserRepository _appUserRepository;

        public UrlsController(ILogger<UrlsController> logger, IUrlsRepository urlsRepository, IAppUserRepository appUserRepository)
        {
            _logger = logger;
            _urlsRepository = urlsRepository;
            _appUserRepository = appUserRepository;
        }

        public async Task<IActionResult> RedirectToOriginal(string shortUrl)
        {
            var record = await _urlsRepository.GetByShortenedUrlAsync(shortUrl);

            if (record != null)
            {
                return Redirect(record.OriginalUrl);
            }
            else
            {
                return NotFound("URL not found");
            }
        }

        public async Task<IActionResult> ShortURLsTable()
        {
            
            var urls = await _urlsRepository.GetAllUrls();
            return View(urls);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateUrlViewModel creatUrlViewModel)
        {
            if (ModelState.IsValid)
            {               
                var handler = new JwtSecurityTokenHandler();
                var claims = handler.ReadJwtToken(HttpContext.Session.GetString("JWToken")).Claims;
                
                int userCreatorId;
                if(Int32.TryParse(claims.FirstOrDefault(c => c.Type == "userId")?.Value, out userCreatorId))
                {
                    var url = new Url
                    {
                        UserId = userCreatorId,
                        OriginalUrl = creatUrlViewModel.OriginalUrl,
                        ShortUrl = RandomStringGenerator.GenerateRandomString(5)
                    };                    

                    _urlsRepository.Add(url);
                    return RedirectToAction("ShortURLsTable");
                }
                else
                {
                    TempData["ErrorMessage"] = "You can't delete this record";
                    return RedirectToAction("ShortURLsTable");
                }
                
            }
            else
            {
                ModelState.AddModelError("", "Failed to create new Url");
            }

            return View(creatUrlViewModel); ;
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var url = await _urlsRepository.GetUrlById(id);
            if (url == null) return View("Error");

            var editUrlViewModel = new EditUrlViewModel
            {
                OriginalUrl = url.OriginalUrl,
            };

            return View(editUrlViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, EditUrlViewModel editMovieViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit movie");
                return View("Edit", editMovieViewModel);
            }

            var existingUrl = await _urlsRepository.GetUrlByIdNoTrackingAsync(id);

            if (existingUrl != null)
            {

                var url = new Url
                {
                    Id = existingUrl.Id,
                    UserId = 1, // for now
                    OriginalUrl = editMovieViewModel.OriginalUrl,
                    ShortUrl = editMovieViewModel.OriginalUrl,
                };

                _urlsRepository.Update(url);

                return RedirectToAction("Index");
            }

            else return View(editMovieViewModel);
        }

        [Authorize]
        public async Task<IActionResult> ShortURLInfo(int id)
        {
            Url url = await _urlsRepository.GetUrlById(id);
            AppUser user = await _appUserRepository.GetUserById(url.UserId);
            var request = HttpContext.Request;
 
            var detailsUrlViewModel = new DetailsUrlViewModel()
            {
                OriginalUrl = url.OriginalUrl,
                ShortenedUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/{url.ShortUrl}",

                UserName = user.FirstName,
                UserLastName = user.LastName,

                CreatedDate = url.CreatedDate,
            };

            return View(detailsUrlViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var url = await _urlsRepository.GetUrlById(id);
            if (url == null) return View("Error");

            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ReadJwtToken(HttpContext.Session.GetString("JWToken")).Claims;
            
            int userCreatorId;
            if(Int32.TryParse(claims.FirstOrDefault(c => c.Type == "userId")?.Value, out userCreatorId))
            {
                var userCreator = await _appUserRepository.GetUserById(userCreatorId);
                if (userCreator.IsAdmin || url.UserId == userCreator.Id)
                {
                    _urlsRepository.Delete(url);
                    return RedirectToAction("ShortURLsTable");
                }
            }
            TempData["ErrorMessage"] = "You can't delete this record";
            return RedirectToAction("ShortURLsTable");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
