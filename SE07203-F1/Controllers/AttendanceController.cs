using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE07203_F1.Data;
using SE07203_F1.Models;

namespace SE07203_F1.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Bước 1: Danh sách lớp của Giảng viên
        public IActionResult Index()
        {
            // Chỉ Lecturer hoặc Admin mới được vào
            var role = HttpContext.Session.GetString("role");
            if (role != "Lecturer" && role != "Admin") return RedirectToAction("Index", "Home");

            var accountId = HttpContext.Session.GetInt32("id");

            var query = _context.ClassSections
                .Include(c => c.Course)
                .Include(c => c.Lecturer)
                .AsQueryable();

            if (role == "Lecturer")
            {
                // Tìm LecturerId dựa trên AccountId
                var lecturer = _context.Lecturers.FirstOrDefault(l => l.AccountId == accountId);
                if (lecturer != null)
                {
                    query = query.Where(c => c.LecturerId == lecturer.Id);
                }
            }

            return View(query.ToList());
        }

        // Bước 2: Danh sách các buổi học (Sessions) của lớp đó
        public IActionResult ListSessions(int classId)
        {
            try
            {
                var sessions = _context.ClassSessions
                    .Where(s => s.ClassSectionId == classId)
                    .OrderBy(s => s.Date) // Sắp xếp theo ngày
                    .ToList();

                ViewBag.ClassId = classId;
                return View(sessions);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                // Đây là FIX nhanh để debug lỗi DB mà không crash app
                ViewBag.Error = "Lỗi Database: Bảng ClassSessions có thể bị thiếu hoặc thiếu cột ClassSectionId. Vui lòng kiểm tra lại SQL Script.";
                ViewBag.SqlErrorMessage = ex.Message;
                // Trả về một View rỗng hoặc trang lỗi
                return View(new List<ClassSession>());
            }
        }

        // Bước 3: Giao diện Điểm danh (GET)
        public async Task<IActionResult> TakeAttendance(int sessionId)
        {
            var session = await _context.ClassSessions.FindAsync(sessionId);
            if (session == null) return NotFound();

            // Lấy danh sách sinh viên CỦA LỚP ĐÓ
            // SỬA LỖI: Include thêm Account để lấy Username làm mã sinh viên
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .ThenInclude(s => s.Account)
                .Where(e => e.ClassSectionId == session.ClassSectionId)
                .ToListAsync();

            // Lấy dữ liệu điểm danh cũ (nếu có)
            var oldData = await _context.Attendances
                .Where(a => a.AttendDate == session.Date && enrollments.Select(e => e.Id).Contains(a.EnrollmentId))
                .ToListAsync();

            // Chuẩn bị ViewModel
            var model = new AttendanceViewModel
            {
                SessionId = sessionId,
                Date = session.Date,
                Slot = session.Slot,
                Items = enrollments.Select(e => new AttendanceItem
                {
                    EnrollmentId = e.Id,

                    // --- ĐÃ SỬA LỖI TẠI ĐÂY ---
                    // Thay .Name bằng .Fullname (khớp với Model Student của bạn)
                    StudentName = e.Student.Fullname,

                    // Thay .Code bằng Account.Username (kiểm tra null để an toàn)
                    StudentCode = e.Student.Account != null ? e.Student.Account.Username : "No Code",

                    // Nếu đã điểm danh rồi thì lấy status cũ, chưa thì mặc định là "Present" (Có mặt)
                    Status = oldData.FirstOrDefault(a => a.EnrollmentId == e.Id)?.Status ?? "Present"
                }).ToList()
            };

            return View(model);
        }

        // Bước 4: Lưu dữ liệu (POST)
        [HttpPost]
        public async Task<IActionResult> TakeAttendance(AttendanceViewModel model)
        {
            foreach (var item in model.Items)
            {
                // Kiểm tra xem đã có record điểm danh chưa
                var att = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.EnrollmentId == item.EnrollmentId && a.AttendDate == model.Date);

                if (att != null)
                {
                    // Update
                    att.Status = item.Status;
                    _context.Update(att);
                }
                else
                {
                    // Insert mới
                    _context.Attendances.Add(new Attendance
                    {
                        EnrollmentId = item.EnrollmentId,
                        AttendDate = model.Date,
                        Status = item.Status
                    });
                }
            }
            await _context.SaveChangesAsync();

            // Quay lại danh sách buổi học
            var session = await _context.ClassSessions.FindAsync(model.SessionId);
            return RedirectToAction("ListSessions", new { classId = session.ClassSectionId });
        }
    }
}