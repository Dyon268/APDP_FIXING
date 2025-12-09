using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE07203_F1.Data;
using SE07203_F1.Models;

namespace SE07203_F1.Controllers
{
    public class ClassSectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassSectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. Danh sách lớp ---
        public async Task<IActionResult> Index(string searchString)
        {
            var role = HttpContext.Session.GetString("role");
            if (string.IsNullOrEmpty(role)) return RedirectToAction("Index", "Login");
            if (role != "Admin" && role != "Lecturer" && role != "Student") return RedirectToAction("Index", "Home");

            // Bắt đầu truy vấn
            var sectionsQuery = _context.ClassSections
                                        .Include(cs => cs.Course)
                                        .Include(cs => cs.Lecturer)
                                        .AsQueryable();

            // Logic Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                sectionsQuery = sectionsQuery.Where(s =>
                    s.Code.ToLower().Contains(searchString) ||
                    s.Room.ToLower().Contains(searchString) ||
                    s.Course.Name.ToLower().Contains(searchString) ||
                    s.Lecturer.Fullname.ToLower().Contains(searchString));
            }

            var sections = await sectionsQuery.ToListAsync();

            ViewData["CurrentFilter"] = searchString;
            return View(sections);
        }

        // --- 2. Form Tạo mới (GET) ---
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin") return RedirectToAction("Index", "Home");

            ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name");
            ViewBag.Lecturers = new SelectList(_context.Lecturers, "Id", "Fullname");

            return View(); // Trả về View rỗng để View tự tạo ViewModel
        }

        // --- 3. Xử lý Tạo & Xếp lịch (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Sửa lỗi Model (Hình a64113): Tham số BẮT BUỘC phải là ViewModel
        public async Task<IActionResult> Create(CreateClassViewModel model)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin") return RedirectToAction("Index", "Home");

            try
            {
                // B1: Lưu Lớp học
                var classSection = new ClassSection
                {
                    Code = model.Code,
                    CourseId = model.CourseId,
                    LecturerId = model.LecturerId,
                    Room = model.Room
                };

                _context.ClassSections.Add(classSection);
                await _context.SaveChangesAsync();

                // B2: Lưu Lịch học (Tự động)
                if (model.DaysOfWeek != null && model.DaysOfWeek.Any())
                {
                    var current = model.StartDate;
                    while (current <= model.EndDate)
                    {
                        if (model.DaysOfWeek.Contains((int)current.DayOfWeek))
                        {
                            var session = new ClassSession
                            {
                                ClassSectionId = classSection.Id, // Khóa ngoại đã được gán
                                Date = current,
                                Slot = model.Slot,
                                Room = model.Room
                            };
                            _context.ClassSessions.Add(session);
                        }
                        current = current.AddDays(1);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // === KHẮC PHỤC LỖI QUAN TRỌNG TẠI ĐÂY ===
                // Nếu lỗi, phải load lại Dropdown
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message + (ex.InnerException != null ? " (" + ex.InnerException.Message + ")" : "");
                ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name");
                ViewBag.Lecturers = new SelectList(_context.Lecturers, "Id", "Fullname");

                // TRẢ VỀ ĐÚNG 'model' (ViewModel) -> View sẽ không bị lỗi mismatch nữa
                return View(model);
            }
        }

    // Trong ClassSectionController.cs

// --- 5. Form Sửa/Xếp lịch (GET) ---
public async Task<IActionResult> Edit(int? id)
        {
            // ... (Kiểm tra Role, NotFound)

            var classSection = await _context.ClassSections
                .Include(cs => cs.ClassSessions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (classSection == null) return NotFound();

            // 💡 Ánh xạ sang ViewModel để giữ lại dữ liệu lịch cũ nếu có
            var model = new ClassSectionEditViewModel
            {
                Id = classSection.Id,
                Code = classSection.Code,
                Room = classSection.Room,
                CourseId = classSection.CourseId,
                LecturerId = classSection.LecturerId,

                // Khởi tạo giá trị lịch mặc định hoặc dựa trên dữ liệu lớp học hiện có
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3),
                Slot = 1, // Mặc định Slot 1
                DaysOfWeek = new int[] { 2, 4 } // Mặc định Thứ 3, Thứ 5
            };

            ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name", classSection.CourseId);
            ViewBag.Lecturers = new SelectList(_context.Lecturers, "Id", "Fullname", classSection.LecturerId);

            return View(model);
        }
        // Trong ClassSectionController.cs

        // --- 6. Xử lý Sửa/Xếp lịch (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Dùng ViewModel mới
        public async Task<IActionResult> Edit(ClassSectionEditViewModel model)
        {
            // ... (Kiểm tra Role)

            if (ModelState.IsValid)
            {
                try
                {
                    // B1: Cập nhật thông tin Lớp học
                    var classSection = await _context.ClassSections.FindAsync(model.Id);
                    if (classSection == null) return NotFound();

                    classSection.Code = model.Code;
                    classSection.Room = model.Room;
                    classSection.CourseId = model.CourseId;
                    classSection.LecturerId = model.LecturerId;

                    _context.Update(classSection);
                    await _context.SaveChangesAsync();

                    // B2: Xóa Lịch học cũ (Cần thiết trước khi xếp lịch mới)
                    var oldSessions = _context.ClassSessions.Where(cs => cs.ClassSectionId == model.Id);
                    _context.ClassSessions.RemoveRange(oldSessions);
                    await _context.SaveChangesAsync();

                    // B3: Lưu Lịch học mới (Tương tự như trong hàm Create)
                    if (model.DaysOfWeek != null && model.DaysOfWeek.Any())
                    {
                        var current = model.StartDate;
                        while (current <= model.EndDate)
                        {
                            if (model.DaysOfWeek.Contains((int)current.DayOfWeek))
                            {
                                var session = new ClassSession
                                {
                                    ClassSectionId = classSection.Id,
                                    Date = current,
                                    Slot = model.Slot,
                                    Room = model.Room // Lấy từ form mới
                                };
                                _context.ClassSessions.Add(session);
                            }
                            current = current.AddDays(1);
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "System Error: " + ex.Message;
                }
            }

            // Tải lại Dropdown nếu lỗi
            ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name", model.CourseId);
            ViewBag.Lecturers = new SelectList(_context.Lecturers, "Id", "Fullname", model.LecturerId);
            return View(model);
        }
        // Inside ClassSectionController.cs

        // --- 7. Handle Delete Submission (POST) ---
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin") return RedirectToAction("Index", "Home");

            // Retrieve the section, including related entities to ensure proper deletion
            var classSection = await _context.ClassSections
                .Include(cs => cs.ClassSessions)
                .Include(cs => cs.Enrollments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (classSection == null) return NotFound();

            // 💡 IMPORTANT: Manually remove related records (Sessions, Enrollments)
            // This is necessary if cascade delete is not configured in the database.
            _context.ClassSessions.RemoveRange(classSection.ClassSessions);
            _context.Enrollments.RemoveRange(classSection.Enrollments);

            // Remove the ClassSection itself
            _context.ClassSections.Remove(classSection);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}