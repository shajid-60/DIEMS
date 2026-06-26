using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIEMS.Data;
using DIEMS.Models;

namespace DIEMS.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly VolunteerRepository _repo;
        private readonly DisasterRepository _disasterRepo;

        public VolunteerController(VolunteerRepository repo, DisasterRepository disasterRepo)
        {
            _repo = repo;
            _disasterRepo = disasterRepo;
        }

        private bool CheckAuth()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var list = _repo.GetAllVolunteers();
            return View(list);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var v = _repo.GetVolunteerById(id);
            if (v == null) return NotFound();

            ViewBag.Assignments = _repo.GetAssignments(id);
            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            
            return View(v);
        }

        [HttpPost]
        public IActionResult Assign(VolunteerAssignment assignment)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                _repo.InsertAssignment(assignment);
            }
            return RedirectToAction("Details", new { id = assignment.VolunteerId });
        }
    }
}
