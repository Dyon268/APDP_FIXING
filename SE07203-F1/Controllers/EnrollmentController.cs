using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE07203_F1.Data;
using SE07203_F1.Models;

namespace SE07203_F1.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Lecturer" && role != "Student")
                return Forbid();

            var enrollments = _context.Enrollments
                                .Include(e => e.Student)
                                .Include(e => e.ClassSection)
                                .ThenInclude(cs => cs.Course)
                                .ToList();
            return View(enrollments);
        }

        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Student")
                return Forbid();

            ViewBag.Students = _context.Students.ToList();
            ViewBag.ClassSections = _context.ClassSections
                                    .Include(cs => cs.Course)
                                    .Include(cs => cs.Lecturer)
                                    .ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Enrollment enrollment)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Student")
                return Forbid();

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
