using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Law_Office_Management.Models;

namespace Law_Office_Management.Controllers
{
    public class AttendanceRecordsController : Controller
    {
        private readonly MyDbContext _context;

        public AttendanceRecordsController(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var records = await _context.AttendanceRecords
                .Include(a => a.Lawyer)
                .ToListAsync();
            return View(records);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.AttendanceRecords
                .Include(a => a.Lawyer)
                .FirstOrDefaultAsync(m => m.AttendanceId == id);

            if (record == null) return NotFound();

            return View(record);
        }

        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (role == "Admin")
            {
                ViewData["LawyerList"] = _context.Lawyers.ToList();
            }
            else if (role == "Lawyer")
            {
                var lawyer = _context.Lawyers.FirstOrDefault(l => l.UserId == userId);
                if (lawyer == null) return Unauthorized();
                ViewData["LawyerList"] = new List<Lawyer> { lawyer };
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendanceRecord record)
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (role == "Lawyer")
            {
                var lawyer = _context.Lawyers.FirstOrDefault(l => l.UserId == userId);
                if (lawyer == null) return Unauthorized();
                record.LawyerId = lawyer.LawyerId;
            }

            if (ModelState.IsValid)
            {
                record.CheckInTime = DateTime.Now;
                _context.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["LawyerList"] = _context.Lawyers.ToList();
            return View(record);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.AttendanceRecords.FindAsync(id);
            if (record == null) return NotFound();

            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (role == "Lawyer")
            {
                var lawyer = _context.Lawyers.FirstOrDefault(l => l.UserId == userId);
                ViewData["LawyerList"] = new List<Lawyer> { lawyer };
            }
            else
            {
                ViewData["LawyerList"] = _context.Lawyers.ToList();
            }

            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttendanceRecord record)
        {
            if (id != record.AttendanceId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["LawyerList"] = _context.Lawyers.ToList();
            return View(record);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.AttendanceRecords
                .Include(a => a.Lawyer)
                .FirstOrDefaultAsync(m => m.AttendanceId == id);

            if (record == null) return NotFound();

            return View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.AttendanceRecords.FindAsync(id);
            if (record != null)
            {
                _context.AttendanceRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> MarkCheckOut(int id)
        {
            var record = await _context.AttendanceRecords
                .Include(r => r.Lawyer)
                .FirstOrDefaultAsync(r => r.AttendanceId == id);

            if (record == null || record.CheckOutTime != null)
                return NotFound();

            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            // المحامي ما يعمل CheckOut إلا لنفسه
            if (role == "Lawyer" && record.Lawyer?.UserId != userId)
                return Unauthorized();

            record.CheckOutTime = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Check-out successful.";
            return RedirectToAction("Index");
        }

    }
}