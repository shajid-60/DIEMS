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

        [HttpGet]
        public IActionResult Index()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            
            ViewBag.Resources = _repo.GetAllResources();
            ViewBag.Critical = _repo.GetCriticalResources();
            ViewBag.DistributionLog = _repo.GetDistributionLog();
            
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            ViewBag.Categories = _repo.GetResourceCategories();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Resource r)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
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
            dist.DistributedBy = HttpContext.Session.GetInt32("UserId") ?? 1;

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
