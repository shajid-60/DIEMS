using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIEMS.Data;
using DIEMS.Models;

namespace DIEMS.Controllers
{
    public class DisasterController : Controller
    {
        private readonly DisasterRepository _repo;

        public DisasterController(DisasterRepository repo)
        {
            _repo = repo;
        }

        private bool CheckAuth()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var list = _repo.GetAllDisasters();
            return View(list);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var d = _repo.GetDisasterById(id);
            if (d == null) return NotFound();

            // Calculate live damage calling Oracle function
            d.EstimatedDamage = _repo.CalculateDamage(id);

            ViewBag.AffectedAreas = _repo.GetAffectedAreas(id);
            return View(d);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            ViewBag.Types = _repo.GetDisasterTypes();
            ViewBag.SeverityLevels = _repo.GetSeverityLevels();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Disaster d)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            d.ReportedBy = HttpContext.Session.GetInt32("UserId");
            
            if (ModelState.IsValid)
            {
                _repo.InsertDisaster(d);
                return RedirectToAction("Index");
            }

            ViewBag.Types = _repo.GetDisasterTypes();
            ViewBag.SeverityLevels = _repo.GetSeverityLevels();
            return View(d);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var d = _repo.GetDisasterById(id);
            if (d == null) return NotFound();

            ViewBag.Types = _repo.GetDisasterTypes();
            ViewBag.SeverityLevels = _repo.GetSeverityLevels();
            return View(d);
        }

        [HttpPost]
        public IActionResult Edit(Disaster d)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                _repo.UpdateDisaster(d);
                return RedirectToAction("Details", new { id = d.DisasterId });
            }

            ViewBag.Types = _repo.GetDisasterTypes();
            ViewBag.SeverityLevels = _repo.GetSeverityLevels();
            return View(d);
        }

        [HttpPost]
        public IActionResult AddAffectedArea(AffectedArea area)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                _repo.InsertAffectedArea(area);
            }
            return RedirectToAction("Details", new { id = area.DisasterId });
        }
    }
}
