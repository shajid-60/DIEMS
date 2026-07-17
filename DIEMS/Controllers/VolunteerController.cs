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

        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");
            
            return View();
        }

        [HttpPost]
        public IActionResult Create(Volunteer v)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");
            
            // If the Admin is registering themselves (e.g. from the Admin checkbox), set the UserId
            if (Request.Form["RegisterAsSelf"] == "true")
            {
                v.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            }

            ModelState.Remove("VolunteerId");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("Username");
            ModelState.Remove("Email");
            ModelState.Remove("UserId");
            ModelState.Remove("TotalHoursServed");
            ModelState.Remove("CurrentMission");
            ModelState.Remove("BloodGroup");
            
            if (ModelState.IsValid)
            {
                _repo.InsertVolunteer(v);
                TempData["SuccessMessage"] = "Volunteer registered successfully!";
                return RedirectToAction("Index");
            }
            
            // If we get here, model state is invalid. Let's return the view so we can see the validation summary.
            return View(v);
        }

        [HttpPost]
        public IActionResult Assign(VolunteerAssignment assignment)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            int assignedByUserId = HttpContext.Session.GetInt32("UserId") ?? 1;

            if (ModelState.IsValid)
            {
                _repo.InsertAssignment(assignment, assignedByUserId);
            }
            return RedirectToAction("Details", new { id = assignment.VolunteerId });
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            bool success = _repo.DeleteVolunteer(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Volunteer deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete volunteer. They might have active assignments.";
            }

            return RedirectToAction("Index");
        }
    }
}
