using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class LawyersController : Controller
    {
        private readonly MyDbContext _context;

        public LawyersController(MyDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            return View(await _context.Lawyers.Include(l => l.User).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var lawyer = await _context.Lawyers
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LawyerId == id);
            if (lawyer == null) return NotFound();

            return View(lawyer);
        }

        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lawyer lawyer, string Username, string Password)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid) return View(lawyer);

            // create user
            var user = new User
            {
                Username = Username,
                PasswordHash = Password,
                Role = "Lawyer",
                Email = lawyer.Email,
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // link to lawyer
            lawyer.UserId = user.UserId;
            _context.Lawyers.Add(lawyer);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Lawyer created successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var lawyer = await _context.Lawyers.FindAsync(id);
            if (lawyer == null) return NotFound();

            return View(lawyer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lawyer lawyer)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id != lawyer.LawyerId) return NotFound();

            if (!ModelState.IsValid) return View(lawyer);

            try
            {
                _context.Update(lawyer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Lawyer updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Lawyers.Any(e => e.LawyerId == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var lawyer = await _context.Lawyers
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LawyerId == id);
            if (lawyer == null) return NotFound();

            return View(lawyer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var lawyer = await _context.Lawyers.FindAsync(id);
            if (lawyer != null)
            {
                _context.Lawyers.Remove(lawyer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Lawyer deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
