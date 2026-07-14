using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIEMS.Data;

namespace DIEMS.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly AnalyticsRepository _repo;

        public AnalyticsController(AnalyticsRepository repo)
        {
            _repo = repo;
        }

        private bool CheckAuth()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private string GetRole()
        {
            return HttpContext.Session.GetString("Role") ?? "";
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Official") return RedirectToAction("AccessDenied", "Home");
            return View();
        }

        [HttpGet]
        public IActionResult GetTrendData()
        {
            if (!CheckAuth()) return Challenge();
            var role = GetRole();
            if (role != "Admin" && role != "Official") return Forbid();
            var data = _repo.GetDisasterTrendData();
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetResourceLevels()
        {
            if (!CheckAuth()) return Challenge();
            var role = GetRole();
            if (role != "Admin" && role != "Official") return Forbid();
            var data = _repo.GetResourceCategoryLevels();
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetHospitalStats()
        {
            if (!CheckAuth()) return Challenge();
            var role = GetRole();
            if (role != "Admin" && role != "Official") return Forbid();
            var data = _repo.GetHospitalStatusData();
            return Json(data);
        }
    }
}
