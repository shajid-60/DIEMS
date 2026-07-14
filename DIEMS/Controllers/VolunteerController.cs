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

        private string GetRole()
        {
            return HttpContext.Session.GetString("Role") ?? "";
        }

        [HttpGet]
        public IActionResult Index(string status, string sort)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Official") return RedirectToAction("AccessDenied", "Home");

            var list = _repo.GetFilteredVolunteers(status, sort);
            ViewBag.CurrentStatus = status ?? "ALL";
            ViewBag.CurrentSort = sort ?? "LATEST";
            return View(list);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            var v = _repo.GetVolunteerById(id);
            if (v == null) return NotFound();

            ViewBag.Assignments = _repo.GetAssignments(id);
            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            
            return View(v);
        }

        [HttpPost]
        public IActionResult Create(Volunteer v)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");
            
            ModelState.Remove("VolunteerId");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("Username");
            ModelState.Remove("Email");
            
            if (ModelState.IsValid)
            {
                _repo.InsertVolunteer(v);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Assign(VolunteerAssignment assignment)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                _repo.InsertAssignment(assignment);
            }
            return RedirectToAction("Details", new { id = assignment.VolunteerId });
        }
    }
}
