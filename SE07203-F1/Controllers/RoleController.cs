using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE07203_F1.Data;
using SE07203_F1.Models;
using System.Linq; // Can thiet cho cac ham LINQ

namespace SE07203_F1.Controllers
{
    // Controller nay hien tai dung de QUAN LY VA PHAN QUYEN CHO TUNG ACCOUNT
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. Danh sach Tai khoan (Index) - DA THEM TIM KIEM ---
        public async Task<IActionResult> Index(string searchString)
        {
            var currentRole = HttpContext.Session.GetString("role");
            if (currentRole != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var accounts = _context.Accounts
                                         .Include(a => a.Role)
                                         .AsQueryable(); // Chuyen sang AsQueryable de tim kiem

            // --- LOGIC TIM KIEM BO SUNG ---
            if (!string.IsNullOrEmpty(searchString))
            {
                accounts = accounts.Where(a => a.Fullname.Contains(searchString)
                                            || a.Username.Contains(searchString)
                                            || a.Role.Name.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }
            // -----------------------------

            return View(await accounts.ToListAsync());
        }

        // --- 2. Form Chinh sua Role (GET) ---
        public async Task<IActionResult> EditAssignment(int? id)
        {
            var currentRole = HttpContext.Session.GetString("role");
            if (currentRole != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null) return NotFound();

            var account = await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Id == id);
            if (account == null) return NotFound();

            var currentAccountId = HttpContext.Session.GetInt32("id");
            if (account.Id == currentAccountId)
            {
                ViewBag.Error = "Ban khong the tu chinh sua vai tro (Role) cua tai khoan hien tai.";
            }

            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", account.RoleId);

            return View(account);
        }

        // --- 3. Xu ly Thay doi Role (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignment(int id, int RoleId)
        {
            var currentRole = HttpContext.Session.GetString("role");
            if (currentRole != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var accountToUpdate = await _context.Accounts.FindAsync(id);
            if (accountToUpdate == null) return NotFound();

            var currentAccountId = HttpContext.Session.GetInt32("id");

            if (accountToUpdate.Id == currentAccountId)
            {
                TempData["ErrorMessage"] = "Loi: Ban khong the tu chinh sua vai tro cua chinh minh.";
                return RedirectToAction(nameof(Index));
            }

            accountToUpdate.RoleId = RoleId;
            try
            {
                _context.Accounts.Update(accountToUpdate);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Da cap nhat Role cho tai khoan '{accountToUpdate.Username}' thanh cong. Nguoi dung can dang nhap lai de Role moi co hieu luc.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Loi khi cap nhat Role: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}