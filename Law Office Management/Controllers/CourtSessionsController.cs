using Law_Office_Management.Models;
using Law_Office_Management.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class CourtSessionsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly NotificationService _notificationService;

        public CourtSessionsController(MyDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        private string GetRole() => HttpContext.Session.GetString("Role") ?? "";
        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");

        // ✅ INDEX
        public async Task<IActionResult> Index()
        {
            var role = GetRole();
            var userId = GetUserId();

            var query = _context.CourtSessions
                .Include(s => s.Case)
                .ThenInclude(c => c.Lawyer)
                .AsQueryable();

            if (role == "Lawyer" && userId.HasValue)
            {
                query = query.Where(s => s.Case != null && s.Case.LawyerId == userId.Value);
            }

            var sessions = await query.ToListAsync();
            return View(sessions);
        }

        // ✅ DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.CourtSessions
                .Include(s => s.Case)
                .ThenInclude(c => c.Lawyer)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null) return NotFound();

            return View(session);
        }

        // ✅ CREATE (GET)
        public IActionResult Create()
        {
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            ViewData["Cases"] = _context.Cases.Include(c => c.Lawyer).ToList();
            return View();
        }

        // ✅ CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourtSession session)
        {
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                ViewData["Cases"] = _context.Cases.ToList();
                return View(session);
            }

            _context.CourtSessions.Add(session);
            await _context.SaveChangesAsync();

            var relatedCase = await _context.Cases
                .Include(c => c.Lawyer)
                .FirstOrDefaultAsync(c => c.CaseId == session.CaseId);

            if (relatedCase != null && relatedCase.LawyerId != null)
            {
                await _notificationService.CreateAsync(
                    userId: relatedCase.LawyerId.Value,
                    title: "📅 New Session",
                    message: $"New session added for case: {relatedCase.Title} on {session.SessionDate?.ToString("yyyy-MM-dd")}",
                    type: "Session",
                    relatedCaseId: relatedCase.CaseId
                );
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            if (id == null) return NotFound();

            var session = await _context.CourtSessions.FindAsync(id);
            if (session == null) return NotFound();

            ViewData["Cases"] = _context.Cases.ToList();
            return View(session);
        }

        // ✅ EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourtSession session)
        {
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            if (id != session.SessionId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["Cases"] = _context.Cases.ToList();
                return View(session);
            }

            try
            {
                _context.Update(session);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.CourtSessions.Any(e => e.SessionId == id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            if (id == null) return NotFound();

            var session = await _context.CourtSessions
                .Include(s => s.Case)
                .ThenInclude(c => c.Lawyer)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null) return NotFound();

            return View(session);
        }

        // ✅ DELETE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (GetRole() != "Admin") return RedirectToAction("AccessDenied", "Home");

            var session = await _context.CourtSessions.FindAsync(id);
            if (session != null)
            {
                _context.CourtSessions.Remove(session);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
