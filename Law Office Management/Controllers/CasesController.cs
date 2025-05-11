using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class CasesController : Controller
    {
        private readonly MyDbContext _context;

        public CasesController(MyDbContext context)
        {
            _context = context;
        }

        private string GetRole() => HttpContext.Session.GetString("Role") ?? "";
        private int? GetLawyerId() =>
            _context.Lawyers.FirstOrDefault(l => l.Email == HttpContext.Session.GetString("Username"))?.LawyerId;

        public async Task<IActionResult> Index()
        {
            var role = GetRole();

            if (role == "Admin")
            {
                var allCases = await _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .ToListAsync();
                return View(allCases);
            }
            else if (role == "Lawyer")
            {
                var lawyerId = GetLawyerId();
                var myCases = await _context.Cases
                    .Include(c => c.Client)
                    .Include(c => c.Lawyer)
                    .Where(c => c.LawyerId == lawyerId)
                    .ToListAsync();
                return View(myCases);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }

        public IActionResult Create()
        {
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            ViewData["Clients"] = _context.Clients.ToList();
            ViewData["Lawyers"] = _context.Lawyers.Where(l => l.IsActive == true).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Case @case)
        {
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                ViewData["Clients"] = _context.Clients.ToList();
                ViewData["Lawyers"] = _context.Lawyers.ToList();
                return View(@case);
            }

            _context.Cases.Add(@case);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @case = await _context.Cases.FindAsync(id);
            if (@case == null) return NotFound();

            var role = GetRole();
            var lawyerId = GetLawyerId();

            if (role != "Admin" && @case.LawyerId != lawyerId)
                return RedirectToAction("AccessDenied", "Home");

            ViewData["Clients"] = _context.Clients.ToList();
            ViewData["Lawyers"] = _context.Lawyers.ToList();
            return View(@case);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Case @case)
        {
            if (id != @case.CaseId) return NotFound();

            var role = GetRole();
            var lawyerId = GetLawyerId();

            if (role != "Admin" && @case.LawyerId != lawyerId)
                return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                ViewData["Clients"] = _context.Clients.ToList();
                ViewData["Lawyers"] = _context.Lawyers.ToList();
                return View(@case);
            }

            try
            {
                _context.Update(@case);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cases.Any(e => e.CaseId == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @case = await _context.Cases
                .Include(c => c.Client)
                .Include(c => c.Lawyer)
                .Include(c => c.Documents)
                    .ThenInclude(d => d.UploadedByNavigation)
                .FirstOrDefaultAsync(m => m.CaseId == id);

            if (@case == null) return NotFound();

            var role = GetRole();
            var lawyerId = GetLawyerId();

            if (role != "Admin" && @case.LawyerId != lawyerId)
                return RedirectToAction("AccessDenied", "Home");

            return View(@case);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var @case = await _context.Cases
                .Include(c => c.Client)
                .Include(c => c.Lawyer)
                .FirstOrDefaultAsync(m => m.CaseId == id);

            if (@case == null) return NotFound();

            return View(@case);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @case = await _context.Cases.FindAsync(id);
            if (@case != null)
            {
                _context.Cases.Remove(@case);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
