using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
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

        public async Task<IActionResult> ShortURLsTable()
        {
            var urls = await _urlsRepository.GetAllUrls();
            return View(urls);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateUrlViewModel creatUrlViewModel)
        {
            if (ModelState.IsValid)
            {
                var url = new Url
                {
                    UserId = 1, // for now
                    OriginalUrl = creatUrlViewModel.OriginalUrl,
                    ShortUrl = creatUrlViewModel.OriginalUrl,
                };

                _urlsRepository.Add(url);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to create new Url");
            }

            return View(creatUrlViewModel); ;
        }

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

        public async Task<IActionResult> ShortURLInfo(int id) 
        {
            Url url = await _urlsRepository.GetUrlById(id);
            AppUser user = await _appUserRepository.GetUserById(url.UserId);

            var detailsUrlViewModel = new DetailsUrlViewModel()
            {
                OriginalUrl = url.OriginalUrl,
                ShortenedUrl = url.ShortUrl,

                UserName = user.FirstName,
                UserLastName = user.LastName,

                CreatedDate = url.CreatedDate,
            };

            return View(detailsUrlViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var url = await _urlsRepository.GetUrlById(id);

            if (url == null) return View("Error");

            _urlsRepository.Delete(url);
            return RedirectToAction("Index");
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
