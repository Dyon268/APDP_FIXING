using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE07203_F1.Data;

namespace SE07203_F1.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Roles = _context.Roles.ToList(); // load roles cho dropdown
            ViewBag.ErrorMessage = string.Empty;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, int roleId)
        {
            ViewBag.Roles = _context.Roles.ToList(); // load lại roles nếu lỗi

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);

            if (account == null)
            {
                ViewBag.ErrorMessage = "Username or password is incorrect!";
                return View("Index");
            }

            if (account.RoleId != roleId)
            {
                ViewBag.ErrorMessage = "Selected role does not match your account!";
                return View("Index");
            }

            // lưu session
            HttpContext.Session.SetString("username", account.Username);
            HttpContext.Session.SetString("fullname", account.Fullname);
            HttpContext.Session.SetInt32("id", account.Id);
            HttpContext.Session.SetString("role", account.Role?.Name ?? "Student");

            return RedirectToAction("Index", "Home");
        }
    }
}
