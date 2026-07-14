using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIEMS.Data;
using DIEMS.Models;

namespace DIEMS.Controllers
{
    public class ShelterController : Controller
    {
        private readonly ShelterRepository _repo;
        private readonly VictimRepository _victimRepo;

        public ShelterController(ShelterRepository repo, VictimRepository victimRepo)
        {
            _repo = repo;
            _victimRepo = victimRepo;
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
            var list = _repo.GetFilteredShelters(status, sort);
            ViewBag.Shelters = list;
            ViewBag.Available = _repo.GetAvailableShelters();
            ViewBag.CurrentStatus = status ?? "ALL";
            ViewBag.CurrentSort = sort ?? "LATEST";
            return View(list);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            
            var s = _repo.GetShelterById(id);
            if (s == null) return NotFound();

            ViewBag.Residents = _repo.GetResidents(id);
            ViewBag.AllVictims = _victimRepo.GetAllVictims(); // For checking in residents
            
            return View(s);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Shelter s)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            s.CreatedBy = HttpContext.Session.GetInt32("UserId");
            
            ModelState.Remove("ShelterId");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                _repo.InsertShelter(s);
                return RedirectToAction("Index");
            }
            return View(s);
        }

        [HttpPost]
        public IActionResult CheckIn(ShelterResident resident)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Responder") return RedirectToAction("AccessDenied", "Home");

            resident.CheckedInBy = HttpContext.Session.GetInt32("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    bool success = _repo.CheckInResident(resident);
                    if (!success)
                    {
                        TempData["ErrorMessage"] = "Check-in failed. Please verify capacity.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error: " + ex.Message;
                }
            }
            return RedirectToAction("Details", new { id = resident.ShelterId });
        }

        [HttpPost]
        public IActionResult CheckOut(int srId, int shelterId)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Responder") return RedirectToAction("AccessDenied", "Home");

            int userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            _repo.CheckOutResident(srId, userId);
            return RedirectToAction("Details", new { id = shelterId });
        }
    }
}
