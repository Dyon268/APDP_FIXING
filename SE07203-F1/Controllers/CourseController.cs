using Microsoft.AspNetCore.Mvc;
using SE07203_F1.Data;
using SE07203_F1.Models;

namespace SE07203_F1.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Lecturer" && role != "Student")
                return Forbid();

            var courses = _context.Courses.ToList();
            return View(courses);
        }

        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin")
                return Forbid();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Course course)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin")
                return Forbid();

            _context.Courses.Add(course);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
