using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIEMS.Data;
using DIEMS.Models;

namespace DIEMS.Controllers
{
    public class ReportController : Controller
    {
        private readonly ReportRepository _repo;
        private readonly DisasterRepository _disasterRepo;

        public ReportController(ReportRepository repo, DisasterRepository disasterRepo)
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
            var list = _repo.GetFilteredReports(status, sort);
            ViewBag.CurrentStatus = status ?? "ALL";
            ViewBag.CurrentSort = sort ?? "LATEST";

            // Audit logs only for Admin and Official
            var role = GetRole();
            if (role == "Admin" || role == "Official")
            {
                ViewBag.AuditLogs = _repo.GetAuditLogs();
            }
            else
            {
                ViewBag.AuditLogs = new System.Collections.Generic.List<DIEMS.Models.AuditLog>();
            }
            
            return View(list);
        }

        [HttpPost]
        public IActionResult Submit(IncidentReport r)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            
            ModelState.Remove("ReportId");
            ModelState.Remove("ReportedAt");
            ModelState.Remove("DisasterName");
            ModelState.Remove("AssignedToName");
            
            if (ModelState.IsValid)
            {
                _repo.InsertReport(r);
                return RedirectToAction("Index");
            }
            return View("Create", r);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var r = _repo.GetReportById(id);
            if (r == null) return NotFound();
            return View(r);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Allow anyone to create report (even citizens/anonymous public reports)
            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            return View();
        }

        [HttpPost]
        public IActionResult Create(IncidentReport r)
        {
            if (ModelState.IsValid)
            {
                _repo.InsertReport(r);
                if (CheckAuth())
                {
                    return RedirectToAction("Index");
                }
                TempData["Message"] = "Report submitted successfully. Thank you for reporting.";
                return RedirectToAction("Create");
            }

            ViewBag.Disasters = _disasterRepo.GetAllDisasters();
            return View(r);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int reportId, string status, string notes)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");
            var role = GetRole();
            if (role != "Admin" && role != "Official") return RedirectToAction("AccessDenied", "Home");

            int userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            var r = new IncidentReport
            {
                ReportId = reportId,
                Status = status,
                AssignedTo = userId,
                ResolutionNotes = notes
            };

            _repo.UpdateReport(r);
            return RedirectToAction("Details", new { id = reportId });
        }
    }
}
