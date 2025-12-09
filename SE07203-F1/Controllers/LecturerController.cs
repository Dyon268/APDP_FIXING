using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE07203_F1.Data;
using SE07203_F1.Models;

namespace SE07203_F1.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LecturerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin")
                return Forbid();

            var lecturers = await _context.Lecturers
                                .Include(l => l.Account)
                                .Include(l => l.Department)
                                .ToListAsync();
            return View(lecturers);
        }

        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin")
                return Forbid();

            ViewBag.Accounts = _context.Accounts.ToList();
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Lecturer lecturer)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin")
                return Forbid();

            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
