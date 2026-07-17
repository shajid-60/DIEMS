using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIEMS.Data;
using DIEMS.Models;

namespace DIEMS.Controllers
{
    public class ResourceController : Controller
    {
        private readonly ResourceRepository _repo;
        private readonly ShelterRepository _shelterRepo;
        private readonly DisasterRepository _disasterRepo;

        public ResourceController(ResourceRepository repo, ShelterRepository shelterRepo, DisasterRepository disasterRepo)
        {
            _repo = repo;
            _shelterRepo = shelterRepo;
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
        public IActionResult Index(string category, string sort)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            
            var list = _repo.GetFilteredResources(category, sort);
            ViewBag.Resources = list;
            ViewBag.Critical = _repo.GetCriticalResources();
            ViewBag.DistributionLog = _repo.GetDistributionLog();
            
            ViewBag.CurrentCategory = category ?? "ALL";
            ViewBag.CurrentSort = sort ?? "LATEST";
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_repo.GetResourceCategories(), "CategoryName", "CategoryName");
            
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            ViewBag.Categories = _repo.GetResourceCategories();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Resource r)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            r.UpdatedBy = HttpContext.Session.GetInt32("UserId");

            if (ModelState.IsValid)
            {
                _repo.InsertResource(r);
                return RedirectToAction("Index");
            }

            ViewBag.Categories = _repo.GetResourceCategories();
            return View(r);
        }

        [HttpGet]
        public IActionResult Distribute(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Responder") return RedirectToAction("AccessDenied", "Home");
            
            var resource = _repo.GetAllResources().Find(x => x.ResourceId == id);
            if (resource == null) return NotFound();

            ViewBag.Resource = resource;
            ViewBag.Shelters = _shelterRepo.GetAllShelters();
            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            
            return View(new ResourceDistribution { ResourceId = id });
        }

        [HttpPost]
        public IActionResult Distribute(ResourceDistribution dist)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Responder") return RedirectToAction("AccessDenied", "Home");

            dist.DistributedBy = HttpContext.Session.GetInt32("UserId") ?? 1;

            // Clear validation for fields not present in the form or populated later
            ModelState.Remove("DistId");
            ModelState.Remove("DistributedBy");
            ModelState.Remove("DistributedAt");
            ModelState.Remove("Status");
            ModelState.Remove("ResourceName");
            ModelState.Remove("CategoryName");
            ModelState.Remove("ShelterName");
            ModelState.Remove("DisasterName");
            ModelState.Remove("DistributedByName");

            if (ModelState.IsValid)
            {
                var result = _repo.DistributeResources(dist);
                if (result.distId > 0)
                {
                    TempData["SuccessMessage"] = result.message;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", result.message);
                }
            }

            var resource = _repo.GetAllResources().Find(x => x.ResourceId == dist.ResourceId);
            ViewBag.Resource = resource;
            ViewBag.Shelters = _shelterRepo.GetAllShelters();
            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            
            return View(dist);
        }
    }
}
