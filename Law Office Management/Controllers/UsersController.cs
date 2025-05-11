using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class UsersController : Controller
    {
        private readonly MyDbContext _context;
        private const int SuperAdminId = 1;

        public UsersController(MyDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        private bool IsProtectedUser(User user)
        {
            if (user.UserId == SuperAdminId)
            {
                TempData["Error"] = "❌ You cannot modify or remove the main admin.";
                return true;
            }
            return false;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            ViewBag.Roles = new List<string> { "Admin", "Lawyer", "Staff" };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<string> { "Admin", "Lawyer", "Staff" };
                return View(user);
            }

            user.IsActive = true;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (IsProtectedUser(user)) return RedirectToAction(nameof(Index));

            user.IsActive = !(user.IsActive);
            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (IsProtectedUser(user)) return RedirectToAction(nameof(Index));

            ViewBag.Roles = new List<string> { "Admin", "Lawyer", "Staff" };
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id != user.UserId) return NotFound();

            if (user.UserId == SuperAdminId)
            {
                TempData["Error"] = "❌ Cannot edit protected admin.";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = Request.Form["IsActive"].Count > 0;

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<string> { "Admin", "Lawyer", "Staff" };
                return View(user);
            }

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.UserId == id)) return NotFound();
                else throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (IsProtectedUser(user)) return RedirectToAction(nameof(Index));

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var user = await _context.Users.FindAsync(id);
            if (user != null && IsProtectedUser(user)) return RedirectToAction(nameof(Index));

            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
