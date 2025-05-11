using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class VerdictsController : Controller
    {
        private readonly MyDbContext _context;

        public VerdictsController(MyDbContext context)
        {
            _context = context;
        }

        // عرض كل الأحكام
        public async Task<IActionResult> Index()
        {
            var verdicts = await _context.Verdicts
                .Include(v => v.Case)
                .ToListAsync();
            return View(verdicts);
        }

        // تفاصيل الحكم
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var verdict = await _context.Verdicts
                .Include(v => v.Case)
                .FirstOrDefaultAsync(v => v.VerdictId == id);

            if (verdict == null) return NotFound();

            return View(verdict);
        }

        // إنشاء حكم
        public IActionResult Create()
        {
            ViewData["Cases"] = _context.Cases.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Verdict verdict)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Cases"] = _context.Cases.ToList();
                return View(verdict);
            }

            _context.Verdicts.Add(verdict);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // تعديل حكم
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var verdict = await _context.Verdicts.FindAsync(id);
            if (verdict == null) return NotFound();

            ViewData["Cases"] = _context.Cases.ToList();
            return View(verdict);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Verdict verdict)
        {
            if (id != verdict.VerdictId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["Cases"] = _context.Cases.ToList();
                return View(verdict);
            }

            try
            {
                _context.Update(verdict);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Verdicts.Any(v => v.VerdictId == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // حذف حكم
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var verdict = await _context.Verdicts
                .Include(v => v.Case)
                .FirstOrDefaultAsync(v => v.VerdictId == id);

            if (verdict == null) return NotFound();

            return View(verdict);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var verdict = await _context.Verdicts.FindAsync(id);
            if (verdict != null)
            {
                _context.Verdicts.Remove(verdict);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
