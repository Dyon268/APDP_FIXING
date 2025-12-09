using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE07203_F1.Data;
using SE07203_F1.Models;
using System.Diagnostics;

namespace SE07203_F1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            // Xóa việc set ViewBag trong Constructor để tránh ghi đè
        }

        public IActionResult Index()
        {
            ViewBag.isConnectedDatabase = _context.Database.CanConnect();
            ViewBag.isLogin = false;
            Account account = new Account();

            // 1. KIỂM TRA TRẠNG THÁI ĐĂNG NHẬP
            if (HttpContext.Session.GetString("username") != null)
            {
                // Người dùng đã đăng nhập (isLogin = true)
                ViewBag.isLogin = true;

                // Load dữ liệu Account từ Session
                account.Fullname = Convert.ToString(HttpContext.Session.GetString("fullname"));
                account.Id = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
                account.Username = Convert.ToString(HttpContext.Session.GetString("username"));

                // Lấy role từ session
                string roleName = HttpContext.Session.GetString("role") ?? "Student";
                account.Role = new Role { Name = roleName };

                ViewBag.role = roleName;

                // TRẢ VỀ VIEW KHI ĐÃ ĐĂNG NHẬP
                // Sẽ trả về Views/Home/Index.cshtml
                return View(account);
            }
            else
            {
                // Người dùng CHƯA đăng nhập (isLogin = false)

                // TRẢ VỀ VIEW KHI CHƯA ĐĂNG NHẬP
                // Sẽ tìm Views/Home/UnauthenticatedIndex.cshtml
                return View("UnauthenticatedIndex");
            }
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            ViewBag.isConnectedDatabase = _context.Database.CanConnect();

            // Tìm account trong database kèm role
            var account = _context.Accounts
                .Include(a => a.Role) // load role
                .FirstOrDefault(a => a.Username == email);

            if (account != null && account.Password == password)
            {
                // Đăng nhập thành công

                // Lưu thông tin session
                HttpContext.Session.SetString("username", account.Username);
                HttpContext.Session.SetString("fullname", account.Fullname);
                HttpContext.Session.SetInt32("id", account.Id);
                HttpContext.Session.SetString("role", account.Role?.Name ?? "Student");

                // CHUYỂN HƯỚNG VỀ TRANG CHỦ (GET) để làm mới giao diện
                // Điều này sẽ kích hoạt lại Action Index() (GET) ở trên
                return RedirectToAction("Index");
            }
            else
            {
                // Đăng nhập thất bại
                ViewBag.isLogin = false;

                // Trả về View UnauthenticatedIndex với thông báo lỗi
                // Bạn cần thêm logic thông báo lỗi vào ViewBag nếu muốn hiển thị trên View
                ViewBag.LoginError = "Invalid username or password.";
                return View("UnauthenticatedIndex");
            }
        }

        // Action Logout đã có sẵn, chỉ sửa để đảm bảo chuyển hướng về Index
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index"); // Chuyển hướng về Action Index() (GET)
        }

        // ... các Actions khác (Privacy, Error) giữ nguyên ...
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}