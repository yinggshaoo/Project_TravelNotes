﻿using Microsoft.AspNetCore.Mvc;
using TravelNotes.Models;

namespace TravelNotes.Controllers
{
    public class BackstageController : Controller
    {
        private readonly object _lockObject = new object();
        //int userID = 1;
        private readonly TravelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public BackstageController(IWebHostEnvironment hostingEnvironment, TravelContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
        public IActionResult Index(int? UserId, string? userName)
        {
            string userId;
            if (!Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                return RedirectToAction("Login", "Member");
            }
            if (!CheckSuperUser())
            {
                return RedirectToAction("Index", "Home");
            }
            var photo = UserId == null ? _context.photo : _context.photo.Where(a => a.UserId == UserId);
            var user = userName == null ? _context.users : _context.users.Where(a => a.Nickname.Contains(userName));
            var photos = from p in photo
                         join u in user
                         on p.UserId equals u.UserId
                         select new
                         {
                             photo = p,
                             user = u
                         };
            ViewBag.photos = photos;
            var article = UserId == null ? _context.article : _context.article.Where(a => a.UserId == UserId);
            var articles = from a in article
                          join u in _context.users
                          on a.UserId equals u.UserId
                          select new
                          {
                              article = a,
                              user = u
                          };
            ViewBag.articles = articles;
            return View();
        }
        public bool CheckSuperUser()
        {
            string userId;
            if (!Request.Cookies.TryGetValue("UsernameCookie", out userId))
            {
                return false;
            }
            users currentUser = _context.users.FirstOrDefault(a => a.UserId == Convert.ToInt32(userId))!;
            if(currentUser.SuperUser == null)
            {
                return false;
            }
            if (currentUser.SuperUser!.ToUpper().Trim() != "Y")
            {
                return false;
            }
            return true;
        }
    }
}