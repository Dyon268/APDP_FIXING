using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Cần thiết cho SelectList
using Microsoft.EntityFrameworkCore;
using SE07203_F1.Data;
using SE07203_F1.Models;
using System.Linq; // Cần thiết cho các hàm LINQ như String.Contains

namespace SE07203_F1.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Student
        // ĐÃ SỬA: Thêm tham số searchString để hỗ trợ tìm kiếm
        public async Task<IActionResult> Index(string searchString)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Lecturer")
                return Forbid();

            var students = _context.Students
                .Include(s => s.Account)
                .Include(s => s.Department)
                .AsQueryable(); // Sử dụng AsQueryable để thực hiện tìm kiếm trên DB

            // --- LOGIC TIM KIEM BỔ SUNG ---
            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Fullname.Contains(searchString)
                                            || s.Account.Username.Contains(searchString) // Tìm theo Mã SV/Username
                                            || s.Department.Name.Contains(searchString)); // Tìm theo Khoa

                // Giữ lại chuỗi tìm kiếm để hiển thị lại trên View
                ViewData["CurrentFilter"] = searchString;
            }
            // -----------------------------

            return View(await students.ToListAsync());
        }

        // GET: /Student/Details/5 (Hàm Xem Tổng quan)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Account)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // GET: /Student/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin") return Forbid();

            // SỬA LỖI: Dùng SelectList để View có thể tạo Dropdown đúng
            ViewBag.Accounts = new SelectList(_context.Accounts.Where(a => !_context.Students.Any(s => s.AccountId == a.Id)), "Id", "Username");
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name");

            return View();
        }

        // POST: /Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Fullname,AccountId,DepartmentId,DateOfBirth,PhoneNumber,Address,Email")] Student student)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin") return Forbid();

            // Cần kiểm tra ModelState.IsValid và load lại ViewBag nếu lỗi xảy ra
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi, phải load lại ViewBag để tránh crash
            ViewBag.Accounts = new SelectList(_context.Accounts.Where(a => !_context.Students.Any(s => s.AccountId == a.Id)), "Id", "Username", student.AccountId);
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name", student.DepartmentId);
            return View(student);
        }

        // GET: /Student/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin") return Forbid();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            ViewBag.Accounts = new SelectList(_context.Accounts
                .Where(a => !_context.Students.Any(s => s.AccountId == a.Id) || a.Id == student.AccountId),
                "Id", "Username", student.AccountId);
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name", student.DepartmentId);

            return View(student);
        }

        // POST: /Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fullname,AccountId,DepartmentId,DateOfBirth,PhoneNumber,Address,Email")] Student student)
        {
            if (id != student.Id) return NotFound();

            var role = HttpContext.Session.GetString("role");
            if (role != "Admin") return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(e => e.Id == student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nếu lỗi, phải load lại ViewBag
            ViewBag.Accounts = new SelectList(_context.Accounts.Where(a => !_context.Students.Any(s => s.AccountId == a.Id) || a.Id == student.AccountId), "Id", "Username", student.AccountId);
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name", student.DepartmentId);
            return View(student);
        }

        // POST: /Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin") return Forbid();

            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}