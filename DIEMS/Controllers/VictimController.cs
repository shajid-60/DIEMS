using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIEMS.Data;
using DIEMS.Models;

namespace DIEMS.Controllers
{
    public class VictimController : Controller
    {
        private readonly VictimRepository _repo;
        private readonly DisasterRepository _disasterRepo;
        private readonly ShelterRepository _shelterRepo;

        public VictimController(VictimRepository repo, DisasterRepository disasterRepo, ShelterRepository shelterRepo)
        {
            _repo = repo;
            _disasterRepo = disasterRepo;
            _shelterRepo = shelterRepo;
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
            var list = _repo.GetFilteredVictims(status, sort);
            ViewBag.CurrentStatus = status ?? "ALL";
            ViewBag.CurrentSort = sort ?? "LATEST";
            return View(list);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var v = _repo.GetVictimById(id);
            if (v == null) return NotFound();

            ViewBag.FamilyMembers = _repo.GetFamilyMembers(id);
            return View(v);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Responder") return RedirectToAction("AccessDenied", "Home");

            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            ViewBag.Shelters = _shelterRepo.GetAllShelters();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Victim v, bool autoAllocate = true)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Responder") return RedirectToAction("AccessDenied", "Home");

            v.RegisteredBy = HttpContext.Session.GetInt32("UserId");

            // Clear ignored fields
            ModelState.Remove("VictimId");
            ModelState.Remove("RegisteredBy");
            ModelState.Remove("RegisteredAt");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("DisasterName");
            ModelState.Remove("ShelterName");
            ModelState.Remove("RegisteredByName");
            ModelState.Remove("Status"); // Handled by DB default

            if (ModelState.IsValid)
            {
                try
                {
                    _repo.InsertVictim(v, autoAllocate);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database Error: " + ex.Message);
                }
            }

            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            ViewBag.Shelters = _shelterRepo.GetAllShelters();
            return View(v);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Responder") return RedirectToAction("AccessDenied", "Home");

            var v = _repo.GetVictimById(id);
            if (v == null) return NotFound();

            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            ViewBag.Shelters = _shelterRepo.GetAllShelters();
            return View(v);
        }

        [HttpPost]
        public IActionResult Edit(Victim v)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Responder") return RedirectToAction("AccessDenied", "Home");
            
            ModelState.Remove("RegisteredBy");
            ModelState.Remove("RegisteredAt");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("DisasterName");
            ModelState.Remove("ShelterName");
            ModelState.Remove("RegisteredByName");
            
            if (ModelState.IsValid)
            {
                _repo.UpdateVictim(v);
                return RedirectToAction("Details", new { id = v.VictimId });
            }

            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            ViewBag.Shelters = _shelterRepo.GetAllShelters();
            return View(v);
        }

        [HttpPost]
        public IActionResult AddFamilyMember(FamilyMember member)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Responder") return RedirectToAction("AccessDenied", "Home");
            
            ModelState.Remove("FmId");
            
            if (ModelState.IsValid)
            {
                _repo.InsertFamilyMember(member);
            }
            return RedirectToAction("Details", new { id = member.VictimId });
        }
    }
}
