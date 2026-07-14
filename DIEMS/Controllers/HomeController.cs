using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIEMS.Data;
using DIEMS.Models;

namespace DIEMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _userRepo;
        private readonly AnalyticsRepository _analyticsRepo;
        private readonly DisasterRepository _disasterRepo;

        public HomeController(UserRepository userRepo, AnalyticsRepository analyticsRepo, DisasterRepository disasterRepo)
        {
            _userRepo = userRepo;
            _analyticsRepo = analyticsRepo;
            _disasterRepo = disasterRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            // Compute SHA256 hash
            string hash = ComputeSha256(password);
            var user = _userRepo.ValidateLogin(username, hash);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("Role", user.RoleName);
                HttpContext.Session.SetString("District", user.District ?? "National");

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid username or password, or account deactivated.";
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var stats = _analyticsRepo.GetDashboardSummary();
            ViewBag.ActiveDisasters = _disasterRepo.GetActiveDisasters();
            
            return View(stats);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }
            return View("~/Views/Shared/AccessDenied.cshtml");
        }

        private string ComputeSha256(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
