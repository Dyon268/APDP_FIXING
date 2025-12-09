using Microsoft.AspNetCore.Mvc;
using SE07203_F1.Data;
using SE07203_F1.Models;

namespace SE07203_F1.Controllers
{
    public class RegisterController : Controller
    {
        readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.IsError = false;
            ViewBag.Roles = _context.Roles.ToList(); // lấy danh sách role
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAccount(string Username, string Fullname, string Password, int RoleId)
        {
            ViewBag.IsError = false;
            ViewBag.Roles = _context.Roles.ToList(); // để form load lại khi có lỗi
            try
            {
                Account account = new Account
                {
                    Username = Username,
                    Fullname = Fullname,
                    Password = Password,
                    RoleId = RoleId // gán RoleId từ form
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                ViewBag.Success = true;
                ViewBag.Error = string.Empty;
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.IsError = true;
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += " | Inner: " + ex.InnerException.Message;
                }
                ViewBag.Error = message;
                return View("Index");
            }
        }
    }
}
