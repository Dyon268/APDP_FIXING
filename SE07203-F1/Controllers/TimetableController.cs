using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE07203_F1.Data;

namespace SE07203_F1.Controllers
{
    public class TimetableController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimetableController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("role");
            var userId = HttpContext.Session.GetInt32("id"); // Id của Account

            if (userId == null) return RedirectToAction("Index", "Login");

            // Lấy danh sách buổi học (Sessions) dựa trên Role
            var sessions = new List<Models.ClassSession>();

            if (role == "Student")
            {
                // 1. Tìm StudentId từ AccountId
                var student = await _context.Students.FirstOrDefaultAsync(s => s.AccountId == userId);
                if (student != null)
                {
                    // 2. Tìm các lớp sinh viên này đã đăng ký (Enrollment)
                    var classIds = await _context.Enrollments
                        .Where(e => e.StudentId == student.Id)
                        .Select(e => e.ClassSectionId)
                        .ToListAsync();

                    // 3. Lấy lịch học của các lớp đó
                    sessions = await _context.ClassSessions
                        .Include(s => s.ClassSection)
                        .ThenInclude(cs => cs.Course)
                        .Include(s => s.ClassSection.Lecturer)
                        .Where(s => classIds.Contains(s.ClassSectionId) && s.Date >= DateTime.Today) // Chỉ lấy lịch tương lai
                        .OrderBy(s => s.Date).ThenBy(s => s.Slot)
                        .ToListAsync();
                }
            }
            else if (role == "Lecturer") // Role tên là "Lecturer" nhé (chỉnh lại nếu bạn dùng Faculty)
            {
                // 1. Tìm LecturerId từ AccountId
                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.AccountId == userId);
                if (lecturer != null)
                {
                    // 2. Lấy lịch các lớp giảng viên này dạy
                    sessions = await _context.ClassSessions
                        .Include(s => s.ClassSection)
                        .ThenInclude(cs => cs.Course)
                        .Where(s => s.ClassSection.LecturerId == lecturer.Id && s.Date >= DateTime.Today)
                        .OrderBy(s => s.Date).ThenBy(s => s.Slot)
                        .ToListAsync();
                }
            }

            return View(sessions);
        }
    }
}