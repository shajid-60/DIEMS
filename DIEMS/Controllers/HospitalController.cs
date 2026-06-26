using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIEMS.Data;
using DIEMS.Models;

namespace DIEMS.Controllers
{
    public class HospitalController : Controller
    {
        private readonly HospitalRepository _repo;
        private readonly DisasterRepository _disasterRepo;

        public HospitalController(HospitalRepository repo, DisasterRepository disasterRepo)
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
            var list = _repo.GetAllHospitals();
            ViewBag.Requests = _repo.GetMedicalRequests();
            return View(list);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var h = _repo.GetHospitalById(id);
            if (h == null) return NotFound();

            ViewBag.Doctors = _repo.GetDoctors(id);
            ViewBag.Ambulances = _repo.GetAmbulances(id);
            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            
            return View(h);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Hospital h)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                _repo.InsertHospital(h);
                return RedirectToAction("Index");
            }
            return View(h);
        }

        [HttpPost]
        public IActionResult RequestMedical(MedicalRequest req)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            req.RequestedBy = HttpContext.Session.GetInt32("UserId") ?? 1;

            if (ModelState.IsValid)
            {
                _repo.InsertMedicalRequest(req);
            }
            return RedirectToAction("Details", new { id = req.HospitalId });
        }
    }
}
