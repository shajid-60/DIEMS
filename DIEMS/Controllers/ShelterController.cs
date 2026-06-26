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

        [HttpGet]
        public IActionResult Index()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            
            ViewBag.Shelters = _repo.GetAllShelters();
            ViewBag.Available = _repo.GetAvailableShelters();
            
            return View();
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
            return View();
        }

        [HttpPost]
        public IActionResult Create(Shelter s)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            s.CreatedBy = HttpContext.Session.GetInt32("UserId");

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
            int userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            _repo.CheckOutResident(srId, userId);
            return RedirectToAction("Details", new { id = shelterId });
        }
    }
}
